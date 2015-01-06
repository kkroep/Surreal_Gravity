var express = require("express");
var http = require("http");
var bodyParser = require("body-parser");
var url = require("url");
var mysql = require("mysql");
var app = express();

var port = 3000;

http.createServer(app).listen(port);

/app.use(express.static(__dirname + "/static"));
app.use(bodyParser.json({strict: true}));

var connection = mysql.createConnection({
  host  : 'localhost',
  user  : 'ewi3620tu2',
  password: 'Jisghav0',
});

connection.connect();
//connection.query('use SURREAL');

app.get("/Players", function (req, res) {
  //res.json(connection.query('select * from todos'));
  var querystring = "SELECT DISTINCT Naam FROM Players;";
  connection.query(querystring, function(err, rows, fields) {
    if (err) throw err;
    res.json(rows);
  });
});

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
