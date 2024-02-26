Real time data for WebGL application.

This solution provides a way to use WebSockets/MQTT real time data in a webGL application.

Avoiding the polling to the server. This could improve the performance of the application, especially if the application manages more devices, using more APIs endpoints.

This solution consists in creating C# Scripts as usually in the Unity side, assign the scripts to GameObjects and create public functions (non-static), which just care about taking parameters and use them (in our case could be the “MoveMir” function).

Once done that, we can build our WebGL (but before, go to Project Settings -> Player -> Publishing Settings and set Compression Format to ‘disabled).

Now we can create a ‘Next.js’ web application. (https://nextjs.org/)

(To use Next.js, is necessary that you have some knowledge of React.js, but on the website you can find also very nice tutorials on how to start with Next.js)
Considering that Next.js is a javascript framework based on React, here we can use React packages.

So in our Next.js application we can install the following package react-unity-webgl in order to embed our Unity WebGL Build inside a dynamic web application. (https://react-unity-webgl.dev/)

Thanks to the ‘sendMessage’ function the library provide us, from the web client application we can call unity functions.

Heres more official info of ‘sendMessage’ javascript method to call unity function, over the ‘Calling Unity scripts functions from JavaScript’ section. (https://docs.unity3d.com/2017.3/Documentation/Manual/webgl-interactingwithbrowserscripting.html)


How all this is useful for web sockets?

Because on the web application side, we can easily get real time data from a web socket service (also MQTT), and every time a message arrives from the server to our application, we can call a Unity Function from ‘outside’.

The Idea is to set up a mqtt and/or websocket channel in order to get MiR real time data, and call the ‘MoveMir’ function we previously created.

Considering there is not a websocket channel yet, I show you that you could polling from website or subscribe to a MQTT Topic in order to get real time data, and pass data into the Unity application.

Side note: MQTT data on web application must be listened to the MQTT endpoint but using the websocket protocol, you can read more info here.

Important note: in development tests, you should bypass the CORS (using a browser plugin/extension), otherwise your browser will block the requests. on deployment, if you will use solution 2, you have to ask to who created the service, to allow your website endpoint to access those data ( here a related issue, which could help you understand better what I mean: https://stackoverflow.com/questions/31276220/cors-header-access-control-allow-origin-missing).

Here chrome extension for bypass CORS. (https://chromewebstore.google.com/detail/allow-cors-access-control/lhobafahddgcelffkeicbaginigeejlf?pli=1)
Here firefox extension for bypass CORS. (https://addons.mozilla.org/en-US/firefox/addon/access-control-allow-origin/)
