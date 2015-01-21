var express = require("express");
var http = require("http");
var bodyParser = require("body-parser");
var url = require("url");
var mysql = require("mysql");
var ejs = require("ejs");
var app = express();

var port = 8082;

http.createServer(app).listen(port);

app.use(express.static(__dirname + "/Static"));
app.use(bodyParser.json({strict: true}));

app.set('view engine', 'ejs');

var connection = mysql.createConnection({
  host  : 'localhost',
  user  : 'ewi3620tu2',
  password: 'Jisghav0',
});

connection.connect();
connection.query('use ewi3620tu2');

app.get("/Players", function (req, res) {
  var querystring = "SELECT * FROM Players;";
  connection.query(querystring, function(err, rows, fields) {
    if (err) throw err;
    res.send(rows);
  });
});

app.get("/Participants", function (req, res) {
  var querystring = "SELECT * FROM Participants;";
  connection.query(querystring, function(err, rows, fields) {
    if (err) throw err;
    res.send(rows);
  });
});

app.get("/Games", function (req, res) {
  var querystring = "SELECT * FROM Games;";
  connection.query(querystring, function(err, rows, fields) {
    if (err) throw err;
    res.send(rows);
  });
});



app.get("/Authenticate",function(req,res){
  var query = url.parse(req.url,true).query;
  var playerName = _escapeString((query["playerName"]!=undefined ? query["playerName"] : "UndefinedName"));
  var playerPassword = _escapeString((query["playerPassword"]!=undefined ? query["playerPassword"] : "UndefinedPassword"));
  //console.log("LOGGING IN playername: " + playerName + "  playerpassword: " + playerPassword);
  var querystring = "SELECT DISTINCT * FROM Players WHERE BINARY Naam = \"" + playerName + "\" AND BINARY Paswoord = \"" + playerPassword + "\";";
  connection.query(querystring,function(err,rows,fields){
    if(err) res.send("Unauthorized");
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
  if(playerName.length>0){
    var playerPassword = _escapeString((query["playerPassword"]!=undefined ? query["playerPassword"] : "UndefinedPassword"));
    //console.log("REGISTERING playername: " + playerName + "  playerpassword: " + playerPassword);
    var querystring = "INSERT INTO `ewi3620tu2`.`Players` (`Naam`, `Paswoord`, `Wanneer`, `PLAYER_Id`) VALUES ('" + playerName + "', '" + playerPassword + "', NOW(), NULL);";
    connection.query(querystring,function(err,result,fields){
      if(err) res.send("Registering failed");
      else res.send("Succesfully Registered");
    });
  }
  else{
    res.send("Can not create empty game account");
  }
});

app.get("/GameRegister",function(req,res){
  console.log("registering game");
  var query = url.parse(req.url,true).query;
  var Server = _escapeString((query["Server"]!=undefined ? query["Server"] : "UndefinedServer"));
  var ServerIDquery = "SELECT PLAYER_Id FROM `Players` WHERE Naam='"+Server+"'";
  var ServerID;
  connection.query(ServerIDquery,function(err,rows,fields){
    if(err) throw err;
    else ServerID = rows[0]["PLAYER_Id"];
    var Winnaar = _escapeString((query["Winnaar"]!=undefined ? query["Winnaar"] : "UndefinedWinnaar"));
    var Finished = _escapeString((query["Finished"]!=undefined ? query["Finished"] : "UndefinedFinished"));
    var Gamemode = _escapeString((query["Gamemode"]!=undefined ? query["Gamemode"] : "UndefinedGamemode"));
    var querystring;
    if(Winnaar == "UndefinedWinnaar"){
      querystring = "INSERT INTO `ewi3620tu2`.`Games` (`Wanneer`, `Server`, `Winnaar`, `Finished`, `Gamemode`, `GAME_Id`) VALUES (NOW(), '"+ServerID+"', NULL, b'0', '"+Gamemode+"', NULL);";
      connection.query(querystring,function(err,result,fields){
        if(err) throw err;
        else res.send("Succesfully Registered Game")
      });
    }
    else{
      var WinnerIDquery = "SELECT PLAYER_Id FROM `Players` WHERE Naam='"+Winnaar+"'";
      var WinnerID;
      connection.query(WinnerIDquery,function(err,rows,fields){
        if(err) throw err;
        else WinnerID = rows[0]["PLAYER_Id"];
        querystring = "INSERT INTO `ewi3620tu2`.`Games` (`Wanneer`, `Server`, `Winnaar`, `Finished`, `Gamemode`, `GAME_Id`) VALUES (NOW(), '"+ServerID+"', '"+WinnerID+"', b'1', '"+Gamemode+"', NULL);";
        connection.query(querystring,function(err,result,fields){
          if(err) throw err;
          else res.send("Succesfully Registered Game")
        });

      });      
    }    
  });
});

app.get("/UnityGlobalInfo", function (req,res){


  var totalGames;
  var mostPlayedName;
  var mostPlayedNumber;
  var mostWinsName;
  var mostWinsNumber;
  var sendstring = "";

  var totalGamesQuery = "SELECT * FROM `totalGames`;";
  connection.query(totalGamesQuery,function(err,rows,fields){
    if(err) res.send("error");
    else totalGames = rows[0]["COUNT(*)"];
    sendstring += totalGames + ",";
    var mostPlayedQuery = "SELECT PlayerNaam, Total FROM `totalPlayerGames` ORDER BY Total DESC;";
    connection.query(mostPlayedQuery,function (err,rows,fields){
      if(err) res.send("error");
      else{ 
        mostPlayedName = rows[0]["PlayerNaam"];
        mostPlayedNumber = rows[0]["Total"];
        sendstring += mostPlayedName + "," + mostPlayedNumber +",";
      }

      var mostWinsQuery = "SELECT WinnaarNaam, Total from `totalPlayerWins` ORDER BY Total DESC;";
      connection.query(mostWinsQuery,function (err,rows,fields){
        if(err) res.send("error");
        else{ 
          mostWinsName = rows[0]["WinnaarNaam"];
          mostWinsNumber = rows[0]["Total"];
          sendstring += mostWinsName + "," + mostWinsNumber;
          res.send(sendstring);
          
        }
      });
    });
  });
});

app.get("/UnityAccountInfo",function (req,res){
  var query = url.parse(req.url,true).query;  
  var totalGamesPlayer;
  var totalWinsPlayer;
  var WinToLose;
  var sendstring = "";
  var playerName = _escapeString((query["playerName"]!=undefined ? query["playerName"] : "UndefinedName"));

  var totalGamesPlayerQuery = "SELECT Total from `totalPlayerGames` WHERE PlayerNaam='"+playerName+"';";
  connection.query(totalGamesPlayerQuery,function (err,rows,fields){
    if(err) res.send("no games played");
    if(rows.length>0){

    totalGamesPlayer = rows[0]["Total"];
    sendstring += totalGamesPlayer + ",";

    var totalWinsPlayerQuery = "SELECT Total from `totalPlayerWins` WHERE WinnaarNaam='"+playerName+"';";
    connection.query(totalWinsPlayerQuery,function (err,rows,fields){
      if(err) res.send("no games played");
      if(rows.length>0){
      totalWinsPlayer = rows[0]["Total"];
      if(totalGamesPlayer>totalWinsPlayer){
        WinToLose = totalWinsPlayer/(totalGamesPlayer-totalWinsPlayer);
      }
      else{
        WinToLose = "Perfect!";
      }
      sendstring += totalWinsPlayer + "," + WinToLose;
      res.send(sendstring);
    }
    else{
      res.send(sendstring + "0,0");
    }
    });
    }
    else{
      res.send("0,0,0")
    }
  });

});

app.get("/ParticipantsRegister",function(req,res){
  var query = url.parse(req.url,true).query;
  var PLAYER = _escapeString((query["PLAYER"]!=undefined ? query["PLAYER"] : "UndefinedPLAYER"));
  var SERVER = _escapeString((query["SERVER"]!=undefined ? query["SERVER"] : "UndefinedSERVER"))

  var PlayerIDquery = "SELECT PLAYER_Id FROM `Players` WHERE Naam='"+PLAYER+"'";
  var PLAYER_Id;
  connection.query(PlayerIDquery,function(err,rows,fields){
    if(err) throw err;
    else PLAYER_Id = rows[0]["PLAYER_Id"];


    var ServerIDquery = "SELECT PLAYER_Id FROM `Players` WHERE Naam='"+SERVER+"'";
    var SERVER_Id;

    connection.query(ServerIDquery,function(err,rows,fields){
      if(err) throw err;
      else SERVER_Id = rows[0]["PLAYER_Id"];
    

      var GAME_Id;

      GameIDquery = "SELECT GAME_Id from `Games` WHERE Server='"+SERVER_Id+"' ORDER BY GAME_Id DESC;"

      connection.query(GameIDquery,function(err,rows,fields){
        if(err) throw err;
        else GAME_Id = rows[0]["GAME_Id"];

        var querystring = "INSERT INTO `ewi3620tu2`.`Participants` (`PLAYER_Id`, `GAME_Id`, `PARTICIPANTS_Id`) VALUES ('"+PLAYER_Id+"', '"+GAME_Id+"', NULL);";

        connection.query(querystring,function(err,rows,fields){
          if(err) throw err;
          else res.send("succesfully logged participant");

        });
      });
    });
  });
});

app.get("/Addwin",function(req,res){
  var query = url.parse(req.url,true).query;
  var playerName = _escapeString((query["playerName"]!=undefined ? query["playerName"] : "UndefinedName"));

  var querystring = "UPDATE `Players` SET Gewonnen = Gewonnen + 1 WHERE Naam='"+playerName+"'";
  connection.query(querystring,function(err,rows,fields){
    if(err) throw err;
    else res.send("succesfully added win to player: "+ playerName);

  });
});

app.get("/Addgespeeld",function(req,res){
  var query = url.parse(req.url,true).query;
  var playerName = _escapeString((query["playerName"]!=undefined ? query["playerName"] : "UndefinedName"));

  var querystring = "UPDATE `Players` SET Gespeeld = Gespeeld + 1 WHERE Naam='"+playerName+"'";
  connection.query(querystring,function(err,rows,fields){
    if(err) throw err;
    else res.send("succesfully added played game to player: "+ playerName);

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

