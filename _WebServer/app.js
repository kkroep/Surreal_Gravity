var express = require("express");
var http = require("http");
var bodyParser = require("body-parser");
var url = require("url");
var mysql = require("mysql");
var app = express();

var port = 8082;

http.createServer(app).listen(port);

app.use(express.static(__dirname + "/Static"));
app.use(bodyParser.json({strict: true}));

var connection = mysql.createConnection({
  host  : 'localhost',
  user  : 'ewi3620tu2',
  password: 'Jisghav0',
});

connection.connect();
connection.query('use ewi3620tu2');

app.get("/Players", function (req, res) {
  //res.json(connection.query('select * from todos'));
  var querystring = "SELECT DISTINCT Naam FROM Players;";
  connection.query(querystring, function(err, rows, fields) {
    if (err) throw err;
    //res.json(rows);
    res.send(rows);
  });
});


app.get("/Authenticate",function(req,res){
  var query = url.parse(req.url,true).query;
  var playerName = (query["playerName"]!=undefined ? query["playerName"] : "UndefinedName");
  var playerPassword = (query["playerPassword"]!=undefined ? query["playerPassword"] : "UndefinedPassword");
  console.log("playername: " + playerName + "  playerpassword: " + playerPassword);
  var querystring = "SELECT DISTINCT * FROM Players WHERE Naam = \"" + playerName + "\" AND Paswoord = \"" + playerPassword + "\";";
  connection.query(querystring,function(err,rows,fields){
    if(err) throw err;
    if(rows.length >= 1){
      res.send("Succesfully Authorized");
    }
    else{
      res.send("Unauthorized");
    }
    
  })


});

app.get("/Register",function(req,res){
  var query = url.parse(req.url,true).query;
  var playerName = (query["playerName"]!=undefined ? query["playerName"] : "UndefinedName");
  var playerPassword = (query["playerPassword"]!=undefined ? query["playerPassword"] : "UndefinedPassword");
  //var querystring = "INSERT INTO 'ewi3620tu2'.'Players' ('Naam', 'Paswoord', 'Gespeeld', 'Gewonnen', 'Wanneer') VALUES ('" + playerName + ", '" + playerPassword + "', '0', '0', NOW());";
  var querystring = "INSERT INTO `ewi3620tu2`.`Players` (`Naam`, `Paswoord`, `Gespeeld`, `Gewonnen`, `Wanneer`, `PLAYER_Id`) VALUES ('" + playerName + "', '" + playerPassword + "', '0', '0', NOW(), NULL);";
  connection.query(querystring,function(err,result,fields){
    if(err) throw err;
    else res.send("Succesfully Registered");
  });

});

/*
app.post("/check", function(req, res) {
  var post = req.body;
  console.log(post);
  var val;
  if (post.value){
    val = "True";
  }
  else {
    val = "False";
  }
  var query = 'UPDATE Todos SET Klaar="' + val + '" WHERE TODO_Id=' + post.key + ";";
  connection.query(query, function(err, rows, fields){
    if (err) throw err;
  });
  res.end();
});


app.get("/new-todo", function (req, res) {
  var url_parts = url.parse(req.url, true);
  var query = url_parts.query;
  var addstring = 'insert into Todos values("' + query["Wat"] + '", "' + query["Wanneer"] + '", "'+ query["Commentaar"] + '", ' + List_Id + ", NULL);";
  connection.query(addstring, function(err,rows,fields) {
    if (err) throw err;
  });
  res.end();
});

app.post("/update-todo", function (req, res) {
  var post = req.body.data;
  var updatestring = 'UPDATE Todos SET Wat="' + post.Wat + '", Wanneer="' + post.Wanneer + '", Commentaar="' + post.Commentaar + '" WHERE TODO_Id=' + post.key + ";";
  connection.query(updatestring, function(err, rows, fields) {
    if (err) throw err;
  });
  res.end();
});

app.post("/delete-todo", function (req, res) {
  var post = req.body.data;
  for (var key in post){
    connection.query('DELETE FROM Todos Where TODO_Id = ' + post[key]);
  }
  res.end();
});

var getRInt = function(bottom, top){
  return Math.floor((Math.random() * top) + bottom);
};
*/