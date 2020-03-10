import * as THREE from 'three';
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls';
import { ImprovedNoise } from 'three/examples/jsm/math/ImprovedNoise';

var getPixels = require("get-pixels");

var image_data, img_width, img_height;

function wait(ms) {
    var deferred = $.Deferred();
    setTimeout(deferred.resolve, ms);

   // We just need to return the promise not the whole deferred.
   return deferred.promise();
}

function getHeightFromPNG() {
    var img = document.getElementById("output");
    var canvas = document.createElement("canvas");
    img_width = canvas.width = img.width;
    img_height = canvas.height = img.height;
    canvas.getContext('2d').drawImage(img, 0, 0, img.width, img.height);
    image_data = canvas.getContext('2d').getImageData(0, 0, img.width, img.height).data;
}


function generateTexture(data, width, height) {
    
    var canvas, canvasScaled, context, image, imageData, vector3, sun, shade;
    
    vector3 = new THREE.Vector3(0, 0, 0);
    
    sun = new THREE.Vector3(1, 1, 1);
    sun.normalize();
    
    canvas = document.createElement('canvas');
    canvas.width = width;
    canvas.height = height;
    
    context = canvas.getContext('2d');
    context.fillStyle = '#000';
    context.fillRect(0, 0, width, height);
    
    image = context.getImageData(0, 0, canvas.width, canvas.height);
    imageData = image.data;
    
    for (var i = 0, j = 0, l = imageData.length; i < l; i += 4, j++) {
        
        vector3.x = data[j - 2] - data[j + 2];
        vector3.y = 2;
        vector3.z = data[j - width * 2] - data[j + width * 2];
        vector3.normalize();
        
        shade = vector3.dot(sun);
        
        imageData[i] = (96 + shade * 128) * (0.5 + data[j] * 0.007);
        imageData[i + 1] = (32 + shade * 96) * (0.5 + data[j] * 0.007);
        imageData[i + 2] = (shade * 96) * (0.5 + data[j] * 0.007);
        
    }
    
    context.putImageData(image, 0, 0);
    
    // Scaled 4x
    
    canvasScaled = document.createElement('canvas');
    canvasScaled.width = width * 4;
    canvasScaled.height = height * 4;
    
    context = canvasScaled.getContext('2d');
    context.scale(4, 4);
    context.drawImage(canvas, 0, 0);
    
    image = context.getImageData(0, 0, canvasScaled.width, canvasScaled.height);
    imageData = image.data;
    
    for (var i = 0, l = imageData.length; i < l; i += 4) {
        
        var v = ~ ~(Math.random() * 5);
        
        imageData[i] += v;
        imageData[i + 1] += v;
        imageData[i + 2] += v;
        
    }
    
    context.putImageData(image, 0, 0);
    
    return canvasScaled;
    
}


// var data = generateHeight(terrain_width, terrain_height);
getHeightFromPNG();

var container = document.getElementById('container');
var width = $(container).width();
var height = 500;

var scene = new THREE.Scene();

var spotLight = new THREE.SpotLight(0xffffff);
spotLight.position.set(0, 0, 10);
spotLight.angle = 3.14 / 4;
spotLight.castShadow = true;

spotLight.shadow.mapSize.width = 10240;
spotLight.shadow.mapSize.height = 10240;

scene.add(spotLight);


var renderer = new THREE.WebGLRenderer();
renderer.setSize(width, height);
renderer.setClearColor(0xffffff, 1);
container.appendChild(renderer.domElement);

var axes = new THREE.AxesHelper(10000);
scene.add(axes);

var geometry = new THREE.PlaneGeometry(7500, 7500, img_width - 1, img_height - 1);
// geometry.rotateX(- Math.PI / 2);

console.log(image_data);
var size = img_width * img_height;
var terrain_data = new Uint32Array(size);
var idx = 0;
var i = 0;
for (var y = 0; y < img_height; y++) {
    for (var x = 0; x < img_width; x++) {
        // terrain_data[i] = 65536 * image_data[idx] + 256 * image_data[idx+1] + image_data[idx+3];
        terrain_data[i] = image_data[idx]
        idx += 4;
        i ++;
    }
}

var camera = new THREE.PerspectiveCamera(60, width / height, 1, 20000);
camera.position.y = terrain_data[img_width / 2 + img_height / 2 * img_width] * 10 + 500;

for (var i = 0, l = geometry.vertices.length; i < l; i++) {
    // vertices[j] = 0;
    // vertices[j + 2] = 0;
    geometry.vertices[i].z = terrain_data[i] * 10;

}
console.log(terrain_data);
console.log(geometry.vertices);
var texture = new THREE.CanvasTexture(generateTexture(terrain_data, img_width, img_height));
texture.wrapS = THREE.ClampToEdgeWrapping;
texture.wrapT = THREE.ClampToEdgeWrapping;

var material = new THREE.MeshPhongMaterial({
    color: 0xdddddd, 
    wireframe: true
  });

var mesh = new THREE.Mesh(geometry, new THREE.MeshBasicMaterial({ map: texture }));
// var mesh = new THREE.Mesh(geometry, material);
scene.add(mesh);

var controls = new OrbitControls(camera, renderer.domElement);



var animate = function () {
    requestAnimationFrame(animate);
    controls.update();
    renderer.render(scene, camera);
};

animate();
