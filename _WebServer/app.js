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
  var playerName = _escapeString((query["playerName"]!=undefined ? query["playerName"] : "UndefinedName"));
  var playerPassword = _escapeString((query["playerPassword"]!=undefined ? query["playerPassword"] : "UndefinedPassword"));
  console.log("LOGGING IN playername: " + playerName + "  playerpassword: " + playerPassword);
  var querystring = "SELECT DISTINCT * FROM Players WHERE Naam = \"" + playerName + "\" AND Paswoord = \"" + playerPassword + "\";";
  connection.query(querystring,function(err,rows,fields){
    if(err) throw err;
    if(rows.length >= 1){
      res.send("Succesfully Authorized");
    }
    else{
      res.send("Unauthorized");
    }    
  });
});

app.get("/Register",function(req,res){
  var query = url.parse(req.url,true).query;
  var playerName = _escapeString((query["playerName"]!=undefined ? query["playerName"] : "UndefinedName"));
  var playerPassword = _escapeString((query["playerPassword"]!=undefined ? query["playerPassword"] : "UndefinedPassword"));
  console.log("REGISTERING playername: " + playerName + "  playerpassword: " + playerPassword);
  var querystring = "INSERT INTO `ewi3620tu2`.`Players` (`Naam`, `Paswoord`, `Gespeeld`, `Gewonnen`, `Wanneer`, `PLAYER_Id`) VALUES ('" + playerName + "', '" + playerPassword + "', '0', '0', NOW(), NULL);";
  connection.query(querystring,function(err,result,fields){
    if(err) throw err;
    else res.send("Succesfully Registered");
  });

});

app.get("/GameRegister",function(req,res){
  var query = url.parse(req.url,true).query;
  var Server = _escapeString((query["Server"]!=undefined ? query["Server"] : "UndefinedServer"));
  var Winnaar = _escapeString((query["Winnaar"]!=undefined ? query["Winnaar"] : "UndefinedWinnaar"));
  var Finished = _escapeString((query["Finished"]!=undefined ? query["Finished"] : "UndefinedFinished"));
  var Gamemode = _escapeString((query["Gamemode"]!=undefined ? query["Gamemode"] : "UndefinedGamemode"));

  var querystring = "INSERT INTO `ewi3620tu2`.`Games` (`Wanneer`, `Server`, `Winnaar`, `Finished`, `Gamemode`, `GAME_Id`) VALUES (NOW(), '"+Server+"', '"+Winnaar+"', b'"+Finished+"', '"+Gamemode+"', NULL);";

  connection.query(querystring,function(err,result,fields){
    if(err) throw err;
    else res.send("Succesfully Registered Game")
  });
});

app.get("ParticipantsRegister",function(req,res){
  var query = url.parse(req.url,true).query;
  var PLAYER_Id = _escapeString((query["PLAYER_Id"]!=undefined ? query["PLAYER_Id"] : "UndefinedPLAYER_Id"));
  var GAME_Id = _escapeString((query["GAME_Id"]!=undefined ? query["GAME_Id"] : "UndefinedGAME_Id"));

  var querystring = "INSERT INTO `ewi3620tu2`.`Participants` (`PLAYER_Id`, `GAME_Id`, `PARTICIPANTS_Id`) VALUES ('"+PLAYER_Id+"', '"+GAME_Id+"', NULL);";
  connection.query(querystring,function(err,result,fields){
    if(err) throw err;
    else res.send("Succesfully logged player and game");
  });
});


function _escapeString (str) {
    return str.replace(/[\0\x08\x09\x1a\n\r"'\\\%]/g, function (char) {
        switch (char) {
            case "\0":
                return "\\0";
            case "\x08":
                return "\\b";
            case "\x09":
                return "\\t";
            case "\x1a":
                return "\\z";
            case "\n":
                return "\\n";
            case "\r":
                return "\\r";
            case "\"":
            case "'":
            case "\\":
            case "%":
                return "\\"+char; // prepends a backslash to backslash, percent,
                                  // and double/single quotes
        }
    });
}

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