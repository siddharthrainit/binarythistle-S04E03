Docker command
----------------------------------------------------------------------------------------
            #docker build -t siddharthrainit/commandsservice .
            #docker run -p 8000:8080 -d siddharthrainit/commandsservice 
            #8000: The port on the host machine that maps to the container's port 8080.
            #Port 8080: The port inside the container where the application is running (container's exposed port).
            #Port 8000: The port on the host machine that maps to the container's port 8080.
            #docker push siddharthrainit/commandsservice
----------------------------------------------------------------------------------------