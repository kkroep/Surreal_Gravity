var main = function () {
  "use strict";

  //setInterval(function () {
    //console.log("Fetching the todo list from the server.");
    //$.getJSON("todos", addTodosToList);
  //}, 10000);
  $.getJSON("Games", addGamesToList);
  $.getJSON("Participants",addParticipantsToList);
  $.getJSON("Players",addPlayersToList);
}
$(document).ready(main);


//Load Todos
var addGamesToList = function (Players) {
  Players.forEach(function(player){
    var tr = document.createElement("tr");
    var $where = $(".Games");
    var td1 = document.createElement("td");
    td1.innerHTML = player["Wanneer"];
    var td2 = document.createElement("td");
    td2.innerHTML = player["Server"];
    var td3 = document.createElement("td");
    td3.innerHTML = player["Winnaar"];
    var td4 = document.createElement("td");
    td4.innerHTML = player["Finished"];
    var td5 = document.createElement("td");
    td5.innerHTML = player["Gamemode"];
    var td6 = document.createElement("td");
    td6.innerHTML = player["GAME_Id"];
    tr.appendChild(td1);tr.appendChild(td2);tr.appendChild(td3);
    tr.appendChild(td4);tr.appendChild(td5);tr.appendChild(td6);
    $where.append(tr);
  })

  //td1.innerHTML = 

  /*
  var todolist = document.getElementById("todo-list");
  var keys = [];
  $(".done").each(function(){keys.push($(this).attr('name'))});
  for (var key in Players) {
    var todo = todos[key];
    if (keys.indexOf(key) < 0){
      //console.log(todo.Wanneer);
      todolist.appendChild(formatTable("hoi1","hoi2", "hoi3")); //todo.Wat, todo.Wanneer, todo.TODO_Id));
    }
  }
  //updateRadiobuttons();
  //updateCheckbuttons();
  */
};

var addParticipantsToList = function (Players) {
  Players.forEach(function(player){
    var tr = document.createElement("tr");
    var $where = $(".Participants");
    var td1 = document.createElement("td");
    td1.innerHTML = player["PLAYER_Id"];
    var td2 = document.createElement("td");
    td2.innerHTML = player["GAME_Id"];
    var td3 = document.createElement("td");
    td3.innerHTML = player["PARTICIPANTS_Id"];

    tr.appendChild(td1);tr.appendChild(td2);tr.appendChild(td3);
    $where.append(tr);
  })


};

var addPlayersToList = function (Players) {
  Players.forEach(function(player){
    var tr = document.createElement("tr");
    var $where = $(".Players");
    var td1 = document.createElement("td");
    td1.innerHTML = player["Naam"];
    var td2 = document.createElement("td");
    td2.innerHTML = player["Paswoord"];
    var td5 = document.createElement("td");
    td5.innerHTML = player["Wanneer"];
    var td6 = document.createElement("td");
    td6.innerHTML = player["PLAYER_Id"];
    tr.appendChild(td1);tr.appendChild(td2);tr.appendChild(td5);tr.appendChild(td6);
    $where.append(tr);
  })

  //td1.innerHTML = 

  /*
  var todolist = document.getElementById("todo-list");
  var keys = [];
  $(".done").each(function(){keys.push($(this).attr('name'))});
  for (var key in Players) {
    var todo = todos[key];
    if (keys.indexOf(key) < 0){
      //console.log(todo.Wanneer);
      todolist.appendChild(formatTable("hoi1","hoi2", "hoi3")); //todo.Wat, todo.Wanneer, todo.TODO_Id));
    }
  }
  //updateRadiobuttons();
  //updateCheckbuttons();
  */
};

/*var getInputFields = function (form){
  var inpwat = form.find("#wat");
  var inpwanneer = form.find("#wanneer");
  var inpcommentaar = form.find("#commentaar");
  console.log(inpcommentaar);
  var returnee = {Wat: inpwat.val(), Wanneer: inpwanneer.val(), Commentaar: inpcommentaar.val()};
  return returnee;
};*/


/*var submitButton = function(){
  $("#submitbutton").click(function(){
    var form = $("#nieuweTodo");
    var input = getInputFields(form);
    $.ajax({
      url: "new-todo",
      type:'GET',
      data: input,
      success: function(msg)
      {
        //alert("yolo");
      }
    });
    updateForm();
  });
}*/

/*var updateForm = function(){
  var todos = $.getJSON("todos", function(todos){
    var table = $("#todo-list");
    table.children("tr").each(function(){
      $(this).remove();
    });
    addTodosToList(todos);
  });
}*/

/*var updateCheckbuttons = function(){
  $('input[type="checkbox"]').each(function(){
    $(this).click(function(){
      var send = {key: parseInt($(this).attr('name')), value: $(this).is(':checked')};
      console.log(send);
      $.ajax({
        url: 'check',
        type: 'POST',
        data: send,
        success: function(msg)
        {
          alert("yolo")
        }
      });
    });
  });
}*/

/*var updateRadiobuttons = function(){
  $('input[type="radio"]').each(function(){
    $(this).click(function(){
      var form = $("#editbox")
      var row = $(this).parent().parent()
      form.find("#wat").val(row.find("#wat").html());
      form.find("#wanneer").val(row.find("#wanneer").html());
      form.find("#editbutton").data("button", {key: parseInt(row.find(".done").attr("name"))});
    });
  });
}*/

/*var editButton = function(){
  $("#editbutton").click(function(){
    var form = $("#editbox");
    var input = getInputFields(form);
    console.log(input);
    if ($("input[name='select']:checked").val()) {
      input.key = parseInt(form.find("#editbutton").data("button").key);
      var jsondata = JSON.stringify({data: input});
      console.log(input);
      $.ajax({
        url: 'update-todo',
        type: 'POST',
        data: jsondata,
        contentType : 'application/json',
        dataType: 'json',
      });
      updateForm();
    }
  });
}*/

/*var deleteButton = function(){
  $("#clearbutton").click(function(){
    var keys = [];
    $('#todo-list').find('input').each(function(){
      if ($(this).is(':checked')){
        keys.push(parseInt(($(this).attr('name'))));
        ($(this).parent().parent().remove());
      }
    });
    console.log(keys);
    var jsondata = JSON.stringify({data: keys});
    $.ajax({
      url: 'delete-todo',
      type: 'POST',
      data: jsondata,
      contentType : 'application/json',
      dataType: 'json',
    });
    updateForm();
  });
}*/

