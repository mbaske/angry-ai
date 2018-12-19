## Angry AI (Unity ML-Agents) - [Video](https://youtu.be/xXTy9xsu25g)

<img src="images/banner.jpg" align="middle" width="1920"/>

This is a little robot battle simulation, made with [Unity Machine Learning Agents](https://github.com/Unity-Technologies/ml-agents).  

The motivation for this project was to experiment with using the [camera's depth texture](https://docs.unity3d.com/Manual/SL-CameraDepthTexture.html) as visual input for training. I also wanted to create a quadruped agent with a rather small action/observation space, compared to the [Crawler](https://www.youtube.com/watch?v=ftLliaeooYI) included in the ml-agents examples. 

The project utilizes the now outdated version 0.5 of ml-agents, which is why the repo includes the Unity C# files. Please note that [Agent.cs](https://github.com/mbaske/angry-ai/blob/master/UnityEnv/Assets/ML-Agents/Scripts/Agent.cs) was changed to allow for a custom texture replacing the camera feed.\
You will have to install the [Unity TensorFlow Plugin](https://github.com/Unity-Technologies/ml-agents/blob/master/docs/Using-TensorFlow-Sharp-in-Unity.md) in order to run the trained models.

Robot design by Lacomap (modified) via [free3d.com](https://free3d.com/user/lacomap)