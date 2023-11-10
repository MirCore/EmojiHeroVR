# EmojiHeroVR

EmojiHeroVR combines virtual reality with affective computing to improve emotion recognition in VR environments. 

This project develops an emotion recognition system tailored to work around the visual limitations of VR headsets, which obscure the upper face.
Applied within an accessible VR game, EmojiHeroVR aims to advance the field of implicit emotion recognition and contribute to an area with significant room for research and development.

## Getting Started

This project has two branches with different VR integration methods. The `main` branch uses the `OpenXR Plugin`, while the `Oculus-Face-Expression` branch is built around the `Oculus Integration (OVR) Plugin`.
The latter supports facial expression tracking with the Meta Quest Pro provided by the OVR plugin. Apart from the VR integration, the branches are not different.

Set up EmojiHeroVR by following these steps:

1. Clone or download the project from the desired branch.
   
2. Open the project in Unity. The minimum tested version is 2022.3.1f1

3. Load the scene appropriate for your branch: `Arcade Scene` for `main` or `Arcade Scene OVR` for `Oculus-Face-Expression`.

4. Access the *EmojiHero Editor Window* via `Window > EmojiHero Editor Window` in Unity.
   - **Primary Webcam**: Select your primary webcam for capturing facial expressions.
   - **Secondary Webcam** [Optional]: This is only utilized in [logging](#logging) to capture additional visual data.
   - **REST API Basepath**: Set this to the basepath of the [FER-Microservice](#requirements) for communication between the application and the facial expression recognition server, e.g., `http://localhost:8000/`.

## Logging

The application logs webcam images, face expressions (with OVR), and FER probabilities to the directory `/EmojiHeroVR/SavedImages/[UserID]/`. Logging is executed after each level to minimize system impact.

The logging structure is as follows:
- FER probabilities and metadata: `[User ID]/logdata.csv`
- Images and face expression data: `[User ID]/[Level Name]/[Emotion]/[timestamp].png` or `[timestamp].json`

## Requirements

- [FER-Microservice](https://github.com/affective-reality-group/facial-expression-recognition-microservice): This microservice provides an HTTP API for Facial Expression Recognition and is essential for the project's functionality.

- [Oculus Integration (OVR) Plugin](https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022): Required for the `Oculus-Face-Expression` branch. Please follow the provided link for installation instructions.

## Credits

All emojis designed by [OpenMoji](https://openmoji.org/) – the open-source emoji and icon project. License: [CC BY-SA 4.0](https://creativecommons.org/licenses/by-sa/4.0/#)

The [RestClient for Unity](https://github.com/proyecto26/RestClient) is developed by [proyecto26](https://github.com/proyecto26). License: [MIT License](https://github.com/proyecto26/RestClient/blob/develop/LICENSE)

8-bit sounds are provided courtesy of [Pixabay](https://pixabay.com/sound-effects/search/8bit/).

## License

This project's source code is licensed under the [MIT License](LICENSE.md).

**Emojis**: The emojis used in this project are designed by [OpenMoji](https://openmoji.org/) and are subject to the [CC BY-SA 4.0 License](https://creativecommons.org/licenses/by-sa/4.0/#). Proper attribution to OpenMoji is required when using these emojis.

By using this project, you agree to comply with the terms of both licenses.
