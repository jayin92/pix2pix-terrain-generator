var r = 50;
var p;
var k = 0.001;

function setup() {
    var canvas = createCanvas(1024, 1024);
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
                    // console.log("test");
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
function inrect(x, y) {
    return x >= 0 && y >= 0 && x < width && y < height;
}
// function mouseWheel(e){
//   if(e.delta>0){k/=1.1;}
//   else {k*=1.1;}
//   console.log(k);
// }