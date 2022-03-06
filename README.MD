### Build It:

```
docker build -t yolo_case_study .
```

### Run It (replace $PORT with the value of your own):

#### On Windows

```
docker run --name=yolo_case_study --restart=always --mount type=bind,source=%cd%\\100MB.bin,target=/app/100MB.bin -p $PORT:80 -d yolo_case_study
```

#### On Linux/MacOS

```
docker run --name=yolo_case_study --restart=always --mount type=bind,source=$(pwd)/100MB.bin,target=/app/100MB.bin -p $PORT:80 -d yolo_case_study
```

### Try It (replace $PORT with the value from above):

```
http://localhost:$PORT/swagger
```
