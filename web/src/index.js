import * as THREE from 'three';

var container = document.getElementById('container');
var width = $(container).width();
var height = 500;

var scene = new THREE.Scene();
var camera = new THREE.PerspectiveCamera(20, width / height, 0.1, 1000);

var renderer = new THREE.WebGLRenderer();
renderer.setSize(width, height);
container.appendChild(renderer.domElement);

var geometry = new THREE.BoxGeometry();
var material = new THREE.MeshBasicMaterial({ color: 0x00ff00 });
var cube = new THREE.Mesh(geometry, material);
scene.add(cube);

camera.position.z = 5;

var animate = function () {
    requestAnimationFrame(animate);

    cube.rotation.x += 0.01;
    cube.rotation.y += 0.01;

    renderer.render(scene, camera);
};

animate();

