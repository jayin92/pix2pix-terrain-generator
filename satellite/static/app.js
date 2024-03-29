var dataJSON = {}

function getBase64(file) {
  var reader = new FileReader();
  reader.readAsDataURL(file);
  reader.onload = function () {
  };
  reader.onerror = function (error) {
  };
}

function getHeightFromPNG() {
  var img = document.getElementById("output");
  var canvas = document.createElement("canvas");
  console.log(img.width, img.height);
  console.log(img.width == 0);
  console.log(img.width === 0);
  if(img.width === 0){
      console.log("test");
      wait(1000);
      var img = document.getElementById("output");
  }
  img_width = canvas.width = img.width;
  img_height = canvas.height = img.height;
  canvas.getContext('2d').drawImage(img, 0, 0, img.width, img.height);
  image_data = canvas.getContext('2d').getImageData(0, 0, img.width, img.height).data;
}


$(document).ready(function (e) {
  $('#image-form').on('submit', (function (e) {
    e.preventDefault();
    if (document.getElementById("three") != null) {
      document.getElementById("three").remove();
    }
    dataJSON["overlay"] = document.getElementById("overlay").value;
    var cas = document.getElementById("defaultCanvas0");
    if (cas != null) {
      dataJSON["file"] = cas.toDataURL("image/png", 1);
      document.getElementById("input").remove();
    } else {
      dataJSON["file"] = document.getElementById("input").src;
    }
    $.ajax({
      url: '/generate',
      type: 'POST',
      dataType: "json",
      data: JSON.stringify(dataJSON),
      contentType: "application/json",
      cache: false,
      processData: false,
      success: function (data) {
        // var script = document.createElement("script");
        // script.src = "/static/bundle.js";
        // scri t.setAttribute("id", "three");
        // document.documentElement.appendChild(script);

        document.getElementById("alertHolder").innerHTML = '<div class="alert alert-success fade show" role="alert">Generate successfully. <a href="/static/gen/' + data['file_name'] + '.png"> Direct Link</a><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button></div>';
        var output = document.createElement("img");
        output.setAttribute("id", "output");
        document.getElementById("img_holder").appendChild(output);
        document.getElementById("output").src = "/static/gen/" + data['file_name'] + ".png";
        output.onload = function() {
          getHeightFromPNG();
          main();
        };
      }
    });
  }));
});

function addBtn() {
  document.getElementById('btnHolder').innerHTML = '<input type="submit" value="Generate" class="btn btn-primary">'
}

$(document).on("click", ".browse", function () {
  var file = $(this).parents().find(".file");
  file.trigger("click");
});


$('input[type="file"]').change(function (e) {
  addBtn();

  var fileName = e.target.files[0].name;
  $("#file").val(fileName);

  var reader = new FileReader();
  reader.onload = function (e) {
    // get loaded data and render thumbnail.
    document.getElementById("input").src = e.target.result;
    document.getElementById("defaultCanvas0").remove();
  };
  // read the image file as a data URL.
  reader.readAsDataURL(this.files[0]);
});


// var el = document.getElementById('c');
// var ctx = el.getContext('2d');

$("#256").click(function () {
  resize(256);
});

$("#512").click(function () {
  resize(512);

});

$("#768").click(function () {
  resize(768);

});

$("#1024").click(function () {
  resize(1024);
});

// ctx.lineWidth = 1;
// ctx.lineJoin = ctx.lineCap = 'round';
// ctx.strokeStyle = 'rgb(100, 100, 100)';
// ctx.shadowBlur = 10;
// ctx.shadowColor = 'rgb(100, 100, 100)';

// var isDrawing, points = [];

// el.onmousedown = function (e) {
//   var rect = el.getBoundingClientRect();
//   isDrawing = true;
//   points.push({ x: e.clientX - rect.left, y: e.clientY - rect.top });
// };

// el.onmousemove = function (e) {
//   if (!isDrawing) return;
//   var rect = el.getBoundingClientRect();

//   // ctx.clearRect(0, 0, ctx.canvas.width, ctx.canvas.height);
//   points.push({ x: e.clientX - rect.left, y: e.clientY - rect.top });

//   ctx.beginPath();
//   ctx.moveTo(points[0].x, points[0].y);
//   for (var i = 1; i < points.length; i++) {
//     ctx.lineTo(points[i].x, points[i].y);
//   }
//   ctx.stroke();
// };

// el.onmouseup = function () {
//   isDrawing = false;
//   points.length = 0;
// };