// work in the map-reduce example db
db = db.getSiblingDB("mrex");

// clean out from previous runs of this sample -- you wouldn't do this in production
db.session.drop();
db.session_stat.drop();

// simulate saving records that log the lengths of user sessions in seconds
db.session.save({ userid: "a", ts: ISODate('2011-11-03 14:17:00'), length: 95 });
db.session.save({ userid: "b", ts: ISODate('2011-11-03 14:23:00'), length: 110 });
db.session.save({ userid: "c", ts: ISODate('2011-11-03 15:02:00'), length: 120 });
db.session.save({ userid: "d", ts: ISODate('2011-11-03 16:45:00'), length: 45 });

db.session.save({ userid: "a", ts: ISODate('2011-11-04 11:05:00'), length: 105 });
db.session.save({ userid: "b", ts: ISODate('2011-11-04 13:14:00'), length: 120 });
db.session.save({ userid: "c", ts: ISODate('2011-11-04 17:00:00'), length: 130 });
db.session.save({ userid: "d", ts: ISODate('2011-11-04 15:37:00'), length: 65 });

/*
For each user, count up the number of sessions, and figure out the average
session length.

Note that to be able to find the average session length, we need to keep a
total of the all the session lengths, and then divide at the end.

We're also going to set this up so that we can repeat the process to get
incremental results over time.
*/

function mapf() {
    emit(this.userid,
         { userid: this.userid, total_time: this.length, count: 1, avg_time: 0 });
}

function reducef(key, values) {
    var r = { userid: key, total_time: 0, count: 0, avg_time: 0 };
    values.forEach(function (v) {
        r.total_time += v.total_time;
        r.count += v.count;
    });
    return r;
}

function finalizef(key, value) {
    if (value.count > 0)
        value.avg_time = value.total_time / value.count;

    return value;
}

/*
Here's the initial run.

The query isn't technically necessary, but is included here to demonstrate
how this is the same map-reduce command that will be issued later to do
incremental adjustment of the computed values.  The query is assumed to run
once a day at midnight.
*/
var mrcom1 = db.runCommand({ mapreduce: "session",
    map: mapf,
    reduce: reducef,
    query: { ts: { $gt: ISODate('2011-11-03 00:00:00')} },
    out: { reduce: "session_stat" },
    finalize: finalizef
});

function saveresults(a) {
    /* append everything from the cursor to the argument array */
    var statcurs = db.session_stat.find();
    while (statcurs.hasNext())
        a.push(statcurs.next());
}

/* save the results into mrres1 */
var mrres1 = [];
saveresults(mrres1);

/* add more session records (the next day) */
db.session.save({ userid: "a", ts: ISODate('2011-11-05 14:17:00'), length: 100 });
db.session.save({ userid: "b", ts: ISODate('2011-11-05 14:23:00'), length: 115 });
db.session.save({ userid: "c", ts: ISODate('2011-11-05 15:02:00'), length: 125 });
db.session.save({ userid: "d", ts: ISODate('2011-11-05 16:45:00'), length: 55 });

/*
Run map reduce again.

This time, the query date is the next midnight, simulating a daily job that
is used to update the values in session_stat.  This can be repeated daily
(or on other periods, with suitable adjustments to the time).
*/
var mrcom2 = db.runCommand({ mapreduce: "session",
    map: mapf,
    reduce: reducef,
    query: { ts: { $gt: ISODate('2011-11-05 00:00:00')} },
    out: { reduce: "session_stat" },
    finalize: finalizef
});

/* save the results into mrres2 */
var mrres2 = [];
saveresults(mrres2);
