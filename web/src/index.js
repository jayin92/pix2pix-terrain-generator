import * as THREE from 'three';
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls';
import { ImprovedNoise } from 'three/examples/jsm/math/ImprovedNoise';

var getPixels = require("get-pixels")

function generateHeight(width, height) {

    var size = width * height, data = new Uint8Array(size),
        perlin = new ImprovedNoise(), quality = 1, z = Math.random() * 100;

    for (var j = 0; j < 4; j++) {

        for (var i = 0; i < size; i++) {

            var x = i % width, y = ~ ~(i / width);
            data[i] += Math.abs(perlin.noise(x / quality, y / quality, z) * quality * 1.75);

        }

        quality *= 5;

    }

    return data;

}
var image_data;
function getHeightFromPNG(file_name) {
    var dir = "/static/gen/";
    var full = dir + file_name + ".png";
    getPixels(full, function (err, pixels) {
        if (err) {
            console.log("Bad image path")
            return
        }
        console.log("got pixels", pixels.shape.slice())
        var width_ = pixels.shape.slice()[0];
        var height_ = pixels.shape.slice()[1];
        var size = width_ * height_;
        console.log(pixels);
        var idx = 0;
        image_data = new Uint32Array(size);
        for (var y = 0; y < height_; y++) {
            for (var x = 0; x < width_; x++) {
                image_data[width_ * y + x] = 65536 * (255 - pixels.data[idx]) + 256 * (255 - pixels.data[idx+1]) + (255 - pixels.data[idx+3]);
                idx += 4;
            }
        }
        console.log(image_data);
    });
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

var terrain_width = 256;
var terrain_height = 256;

// var data = generateHeight(terrain_width, terrain_height);
console.log(getHeightFromPNG(document.getElementById("image_id").innerHTML));
console.log(image_data);
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

var camera = new THREE.PerspectiveCamera(60, width / height, 1, 20000);
camera.position.y = data[terrain_width / 2 + terrain_height / 2 * terrain_width] * 10 + 500;

var renderer = new THREE.WebGLRenderer();
renderer.setSize(width, height);
renderer.setClearColor(0xffffff, 1);
container.appendChild(renderer.domElement);

var axes = new THREE.AxesHelper(10000);
scene.add(axes);

var geometry = new THREE.PlaneBufferGeometry(7500, 7500, terrain_width - 1, terrain_height - 1);
geometry.rotateX(- Math.PI / 2);

var vertices = geometry.attributes.position.array;

for (var i = 0, j = 0, l = vertices.length; i < l; i++ , j += 3) {

    vertices[j + 1] = image_data[i];

}

var texture = new THREE.CanvasTexture(generateTexture(data, terrain_width, terrain_height));
texture.wrapS = THREE.ClampToEdgeWrapping;
texture.wrapT = THREE.ClampToEdgeWrapping;

var mesh = new THREE.Mesh(geometry, new THREE.MeshBasicMaterial({ map: texture }));
scene.add(mesh);

var controls = new OrbitControls(camera, renderer.domElement);



var animate = function () {
    requestAnimationFrame(animate);
    controls.update();
    renderer.render(scene, camera);
};

animate();
