## Synopsis

This project is used for performing screen calibration for cubic displays.

## Code Example

Show what the library does as concisely as possible, developers should be able to figure out **how** your project solves their problem by looking at the code example. Make sure the API you are showing off is obvious, and that your code is short and concise.

To use the software, take pictures of the cube display from a variety of different locations. The image titled screenCheck8x6.png is the style of image that should be rendered no each screen. The matlab script makeCheckScreen can be used to generate an image to fit the resolution of the screens being used.

The images that are taken must each have 3 screens visible (this isn't a requirement for the algorithm but the code expects it). Make sure the screens are set to their brightest and set the camera to a relatively low frame rate. Some sample images are shown in the Sample_Images folder.

The main script is Calibrate_Screens_V2. The script will ask for the basename of the images (for the sample images it's "Grid") as well as the size of the search window when finding corners (20-50 pixels should be fine).

Each image will be negated and presented in a figure. You must click the 4 outer most corners of the pattern on each screen, starting with the top screen. The corners must be clicked in a specific order to get the screen orientation right. The image clickOrder shows the order. Once all 4 corners have been clicked, the program will calculate the location of every intermediate corner. If everything looks good, press enter, if not input 'n' to try again. Once the top screen is good, repeate for the other 2 (the order of the 2 doesn't matter).

Once each screen has been processed, 2 figures will appear showing the results and the transformations are saved in final_Transform variable

## Motivation

A short description of the motivation behind the creation and maintenance of the project. This should explain **why** the project exists.

## Installation

Provide code examples and explanations of how to get the project.

## API Reference

Depending on the size of the project, if it is small and simple enough the reference docs can be added to the README. For medium size to larger projects it is important to at least provide a link to where the API reference docs live.

## Tests

Describe and show how to run the tests with code examples.

## Contributors

Let people know how they can dive into the project, include important links to things like issue trackers, irc, twitter accounts if applicable.

## License

A short snippet describing the license (MIT, Apache, etc.)
