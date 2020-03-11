var r = 75;
var p;
var k = 0.001;

function inrect(x, y) {
    return x >= 0 && y >= 0 && x < width && y < height;
}

function setup(size=512) {
    clear();
    var canvas = createCanvas(size, size);
    background(0);
    canvas.parent('p5-holder');
    p = new Float64Array(width * height);
}

function draw() {
    loadPixels();
    if (mouseIsPressed) {
        for (var x = -r; x < r; x++) {
            for (var y = -r; y < r; y++) {
                if (inrect(x + mouseX, y + mouseY)) {
                    var i = x + mouseX + (y + mouseY) * width;
                    p[i] += 20.0 * (exp(-k * (x * x + y * y)));
                    pixels[i * 4] = p[i];
                    pixels[i * 4 + 1] = p[i];
                    pixels[i * 4 + 2] = p[i];
                }
            }
        }
    }
    updatePixels();
}

function mouseWheel(e) {
    if (e.delta > 0) { k /= 1.1; }
    else { k *= 1.1; }
}