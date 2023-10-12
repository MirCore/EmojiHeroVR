# EmojiHeroVR


## Getting Started

Follow the steps below to get started with EmojiHeroVR:

1. Open the project in Unity, preferably using version 2022.3 or later.

2. Open the *EmojiHero Editor Window* by selecting `Window > EmojiHero Editor Window` in Unity.
  - **Select the primary webcam**: Make sure you select the primary webcam device you want to use for emotion recognition. This webcam will be used to capture the user's facial expressions.
  - **Set the REST API basepath**: Configure the REST API basepath where the EmojiHeroVR application communicates with the [FER-Microservice](#Dependencies). For example, if you are running the server locally, set the basepath to `http://localhost:8000/`.

## Dependencies

- [FER-Microservice](https://github.com/affective-reality-group/facial-expression-recognition-microservice): A microservice providing an HTTP API for Facial Expression Recognition (FER).

## Credits

All emojis designed by [OpenMoji](https://openmoji.org/) – the open-source emoji and icon project. License: [CC BY-SA 4.0](https://creativecommons.org/licenses/by-sa/4.0/#)

The [RestClient for Unity](https://github.com/proyecto26/RestClient) is developed by [proyecto26](https://github.com/proyecto26). License: [MIT License](https://github.com/proyecto26/RestClient/blob/develop/LICENSE)

## License

This project's source code is licensed under the [MIT License](LICENSE.md).

- **Emojis**: The emojis used in this project are designed by [OpenMoji](https://openmoji.org/) and are subject to the [CC BY-SA 4.0 License](https://creativecommons.org/licenses/by-sa/4.0/#). Proper attribution to OpenMoji is required when using these emojis.

By using this project, you agree to comply with the terms of both licenses.
