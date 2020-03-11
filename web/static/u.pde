void setup(){
  size(1024, 1024);
  background(0);
  p=new float[width*height];
}
int r=75;
float p[];
float k=0.001;
void draw(){
  loadPixels();

  if(mousePressed){
    for(int x=-r;x<r;x++){
      for(int y=-r;y<r;y++){
        if(inrect(x+mouseX,y+mouseY)){
          int i=x+mouseX+(y+mouseY)*width;
          p[i]+=20.0*(exp(-k*(x*x+y*y)));
          p[i]=min(255, p[i]);
          pixels[i]=color(p[i]);
        }
      }
    }
  }
  updatePixels();
}
boolean inrect(int x,int y){
  return x>=0&&y>=0&&x<width&&y<height;
}
void mouseWheel(MouseEvent e){
  if(e.getCount()>0)k/=1.1;
  else k*=1.1;
}
