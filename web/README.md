# Scifair web service

## Usage
- Start flask web service:
`python app.py`

- Web interface:
[http://localhost:8888/](http://localhost:8888/)

- Generate fake image:
    `curl -F "file=@/path/to/image/input.png" http://localhost:8888/generate`

    + API will return a json response which looks like this:
    ```
    {
        'file_name': <img_name>
    }
    ```
    + You can get generated image by going to [http://localhost:8888/static/gen/<img_name>.png](http://localhost:8888/static/gen/<img_name>.png)