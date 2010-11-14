#!/usr/bin/perl -w

use Net::Pcap;
use strict;

die "Usage: $0 infile outfile" unless scalar(@ARGV) > 1;

my $err;

open(my $out, '>', $ARGV[1]) or die "cannot open in file: $!";

my $pcap = Net::Pcap::open_offline($ARGV[0], \$err);
Net::Pcap::dispatch($pcap, 0, \&process_pkt, $out);
Net::Pcap::close($pcap);
close($out);

sub process_pkt {
# skip IP, UPD, RTP & RTP specific H.263 headers and write H.263 data to $out
    my($out, $hdr, $pkt) = @_;
    
    my $hdrskip = 54;
    my $bitflags = substr( $pkt, $hdrskip, 1 );
    my $bitflags2 = ((ord $bitflags) & 0xc0);

    if ( $bitflags2 == 0xc0 )		# mode C h.263 header
    	{ $hdrskip += 12; }
    elsif ( $bitflags2 == 0x80 )	# mode B h.263 header
    	{ $hdrskip += 8; }
    elsif ( $bitflags2 == 0x00 )	# mode A h.263 header
    	{ $hdrskip += 4; }

    my $payload = substr($pkt, $hdrskip);

    print $out $payload;
}
