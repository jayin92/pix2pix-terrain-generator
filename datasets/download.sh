PKG_OK=$(dpkg-query -W --showformat='${Status}\n' wget|grep "install ok installed")
echo Checking for wget: $PKG_OK
if [ "" = "$PKG_OK" ]; then
  echo "No wget. Setting up wget."
  sudo apt-get --force-yes --yes install wget
fi

PKG_OK=$(dpkg-query -W --showformat='${Status}\n' unzip|grep "install ok installed")
echo Checking for unzip: $PKG_OK
if [ "" = "$PKG_OK" ]; then
  echo "No unzip. Setting up unzip."
  sudo apt-get --force-yes --yes install unzip
fi


wget https://github.com/jayin92/scifair-datasets/releases/download/v1.0/heightmap.zip
wget https://github.com/jayin92/scifair-datasets/releases/download/v1.0/perlin_for_test.zip
wget https://github.com/jayin92/scifair-datasets/releases/download/v1.0/perlin_image.zip
wget https://github.com/jayin92/scifair-datasets/releases/download/v1.0/simple_poly.zip

unzip heightmap.zip 
unzip perlin_for_test.zip
unzip perlin_image.zip
unzip simple_poly.zip