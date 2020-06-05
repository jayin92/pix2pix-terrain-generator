var r = 250;
var p;
var k = 0.001;
var s;

function setup() {    
    var canvas = createCanvas(256, 256);
    background(0);
    canvas.parent('p5-holder');
    p = new Float64Array(width * height);
    s=0.1/sqrt(k)
}

function draw() {
    loadPixels();
    if (mouseIsPressed) {
        for (var x = -r; x < r; x++) {
            for (var y = -r; y < r; y++) {
                if (inrect(x + mouseX, y + mouseY)) {
                    var i = x + mouseX + (y + mouseY) * width;
                    p[i] +=  s* (exp(-k * (x * x + y * y)));
                    pixels[i * 4] = p[i];
                    pixels[i * 4 + 1] = p[i];
                    pixels[i * 4 + 2] = p[i];
                }
            }
        }
    }
    updatePixels();
}

function resize(size){
    resizeCanvas(size, size);
    clear();
    background(0);
    p = new Float64Array(width * height);
}

function mouseWheel(e) {
    if (e.delta > 0) { k /= 1.1; }
    else { k *= 1.1; }
    s=0.1/sqrt(k)
}

function inrect(x, y) {
    return x >= 0 && y >= 0 && x < width && y < height;
}