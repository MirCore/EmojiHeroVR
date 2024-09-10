# EmojiHeroVR

EmojiHeroVR combines virtual reality with affective computing to improve emotion recognition in VR environments. 

This project develops an emotion recognition system tailored to work around the visual limitations of VR headsets, which obscure the upper face.
Applied within an accessible VR game, EmojiHeroVR aims to advance the field of implicit emotion recognition and contribute to an area with significant room for research and development.

## Getting Started

Set up EmojiHeroVR by following these steps:

1. Clone or download the project.
   
2. Open the project in Unity 6. The minimum tested version is 6000.0.16f1

3. Load the `Arcade Scene` or  `Arcade Scene VR` scene.
  
5. Further settings can be found through the Inspector in the MANAGERS GameObjects.

## Logging

Logging is not currently available in the Senits version. See the [v1.0-study-setup release](https://github.com/MirCore/EmojiHeroVR/releases/tag/v1.0-study-setup) for logging capabilities.

## ONNX Models

- The project utilizes the *Ultra-lightweight face detection model* (version *version-RFB-640*) for face detection: [Ultra-Light-Fast-Generic-Face-Detector](https://github.com/Linzaer/Ultra-Light-Fast-Generic-Face-Detector-1MB)
- For emotion recognition without HMDs the *HSEmotion library* (version *enet_b2_7.pt*) is used: [HSEmotion (High-Speed face Emotion recognition) library](https://github.com/av-savchenko/face-emotion-recognition/tree/main)
- The HMD FER is based on the *EmojiHeroVR Database*: [Emoji-Hero-VR-Database](https://github.com/thorbenortmann/emoji-hero-vr-database)
 
## Credits

All emojis designed by [OpenMoji](https://openmoji.org/) – the open-source emoji and icon project. License: [CC BY-SA 4.0](https://creativecommons.org/licenses/by-sa/4.0/#)

The [RestClient for Unity](https://github.com/proyecto26/RestClient) is developed by [proyecto26](https://github.com/proyecto26). License: [MIT License](https://github.com/proyecto26/RestClient/blob/develop/LICENSE)

8-bit sounds are provided courtesy of [Pixabay](https://pixabay.com/sound-effects/search/8bit/).

## License

This project's source code is licensed under the [MIT License](LICENSE.md).

**Emojis**: The emojis used in this project are designed by [OpenMoji](https://openmoji.org/) and are subject to the [CC BY-SA 4.0 License](https://creativecommons.org/licenses/by-sa/4.0/#). Proper attribution to OpenMoji is required when using these emojis.

By using this project, you agree to comply with the terms of both licenses.
