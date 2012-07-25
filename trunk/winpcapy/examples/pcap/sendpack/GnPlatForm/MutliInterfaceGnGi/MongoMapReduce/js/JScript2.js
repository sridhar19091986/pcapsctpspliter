  /*
  
  { username: "jones",
    likes: 20,
    text: "Hello world!"
  }
  **/

  function() {
    emit( this.username, {count: 1, likes: this.likes} );
  }

 function(key, values) {
    var result = {count: 0, likes: 0};

    values.forEach(function(value) {
      result.count += value.count;
      result.likes += value.likes;
    });

    return result;
  }
