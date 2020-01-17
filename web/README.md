# Scifair web service

## Usage
- Start flask web service:
`FLASK_ENV=development FLASK_APP=app.py flask run`

- Generate fake image:
`curl -F "file=@/path/to/image/real.png" http://localhost:5000 --output output.png`
