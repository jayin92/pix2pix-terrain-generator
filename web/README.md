# Scifair web service

## Usage
- Start flask web service:
`python app.py`

- Web interface:
[http://localhost:8888/](http://localhost:8888/)

- Generate fake image:
`curl -F "file=@/path/to/image/real.png" http://localhost:8888/generate --output output.png`
