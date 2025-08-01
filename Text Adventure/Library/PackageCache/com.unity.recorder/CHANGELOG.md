# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [5.1.0-pre.1] - 2023-10-31
### Added
- Added error message in UI if full output file path exceeds 259 characters.
- The AOV Image Recorder now supports recording multiple AOVs at once and writes multi-part EXR files.
  - Added the option to export AOVs of each frame to a single multi-part EXR file or to multiple EXR files.
  - On upgrade, AOV Recorder settings seamlessly translate the former single AOV drop-down selection into the corresponding new AOV checkbox.
  - Added categories to AOV selection UI for improved conceptual grouping and multi-selection.
  - Revised all AOV labels and added tooltips for each AOV and category.
- The AOV Image Recorder supports additional compression options: Zips, B44, B44a, DWAA and DWAB.
- Added a tutorial in documentation to set up a Unity project with API scripting for starting recordings via a command line.

### Changed
- Removed resolution automatic rounding to the nearest even number when Movie Recorder uses MP4 codec and "Match Window Size" output resolution.
- Removed JPEG export option from the AOV Recorder.
- The Ambient Occlusion AOV was outputting the contents of the occlusion material map. Now, the AOV outputs the result of the Screen Space Ambient Occlusion post-processing pass.
- Removed two unnecessary composited Lighting AOVs (Diffuse Lighting and Specular Lighting) from the exportable AOVs.
- Refactored the documentation around the two recording frameworks available and the different use cases provided as examples.
- Updated the documentation architecture overall to improve navigation and findability.

### Fixed
- Prevent video output from being corrupted when generated via a Recorder Clip run through a Scene loaded in Play mode.
- Ensure the project uses the right RenderPipeline settings at all time: quality RenderPipeline settings or default RenderPipeline settings.
- Ensure that the status messages at the bottom of the Recorder window are always readable even if the window is small.
- Fix the visibility of the Preset icon in Light mode.

## [5.0.0-pre.1] - 2023-05-11
### Changed
- Updated minimum required Unity Editor version to 2023.1.
- Removed obsolete warnings about spot lights when recording accumulation in 2023.2.
- Removed AOV recorder.

### Fixed
- An exception no longer occurs when deleting all but one recorder when the deleted recorders' outputs were incompatible with, or names conflicted with, the remaining recorder.
- An error message is now displayed and the Recorder is prevented from starting if MainCamera is selected while the main camera is missing from the project.
- Prevent the Recorder from capturing audio if there are more than two audio channels because only mono or stereo recording is supported.
- To fix a compression issue, the VP8 target bitrate is now converted to bits per second (bps) instead of kilobits per second (kbps).
- The Physical Camera property now stays enabled when recording with accumulation anti-aliasing.

## [4.0.0] - 2022-06-01
### Fixed
- Better handle error messages when using a Tagged Camera while the tag is missing from the project or not assigned to the camera.
- Make sure to stop the AudioRenderer when all recordings are over.
- Hide the Include Alpha checkbox for Movie Recorders in URP projects.
- Make sure the Recorder Window uses error icons for errors that prevent the recording from starting.
- When accumulation is activated, make sure all recorders check for errors.

## [4.0.0-pre.5] - 2022-04-20
### Added
- Movie Recorder Encoder sample showing how to integrate a custom command line encoder such as FFmpeg.

### Changed
- Only accumulate subframes within the specified recording time/frame range, and not on skipped frames.

### Fixed
- Make sure the Game View UI displays the actual resolution when the Recorder needs to change it.
- Prevent the Recorder from capturing audio when the output format doesn't support it.
- Support ProRes 4444 export on Apple M1 Max CPUs.
- Make sure to get the proper recording duration when capturing accumulation and specifying a recording time range in seconds.
- Fix accumulation frame artifacts when specifying a recording time range in seconds that doesn't start from zero.
- Prevent the Unity Editor from crashing when multiple recorders use different output resolutions.
- Prevent Unity from logging an error in the console when exiting timeline clips with the GIF encoder.

## [4.0.0-pre.4] - 2022-04-19
### Added
- GIF Encoder for the Movie Recorder.
- Accumulation based shadow map filtering for Spot Lights, improving the quality of rendered shadows.
- New option to apply Subpixel Jitter Antialiasing when recording with Accumulation (in HDRP).
- PIZ compression for EXR image sequences.

### Changed
- Remove support for CentOS.
- Allow audio recording while using the Movie Recorder's accumulation feature.
- Improvements to the Accumulation UI parameters:
  - Specify the shutter interval as a normalized interval or an angle.
  - Specify the number of accumulation samples without a maximum limit, directly through a value field.
  - Get the actual count of accumulated frames resulting from the shutter profile convolution.
- Prevent users from recording in odd resolutions with ProRes packed pixel format codecs (e.g. 422HQ).
- Movie Recorder no longer uses the Sync GPU readback code path.
- Use error icons in the Recorder Window for errors that prevent the recording from starting.

### Fixed
- Improved performance for ProRes encoding.
- Fix an issue that prevented the alpha component from being correctly recorded.
- Make sure to generate high bitrate audio tracks when using custom quality encoding in the Unity Media Encoder.
- Make sure the duration of recordings is accurate when capturing accumulation with time intervals in seconds.
- Make sure the CapFPS setting remains reliable when the recording session includes multiple active Recorders.
- Refresh the asset database after recording an image sequence to the Assets folder.
- Prevent users from recording if the Recorder Window combines active recorders with and without accumulation.
- Prevent the Recorder Window from throwing errors when selecting an invalid Encoder.

## [4.0.0-pre.3] - 2021-11-01
### Added
- Added a new "Encoder" drop down with the Unity Media Encoder and ProRes Encoder.
- Added a new public Encoder API to allow users to create their own Encoders for seamless integration into the Movie Recorder.
- Added a new "Custom" quality for the Unity Media Encoder, with different options for H.264 MP4 and VP8 WebM.
- Support accumulation feature in Timeline Recorder Clips.

### Changed
- Updated minimum required Unity Editor version to 2022.1.
- Renamed the "Capture" section to "Input" and the "Format" section to "Output Format".
- Moved the "Include Audio" option to the "Output Format" section of the Recorder Window.
- Ignore "Capture Alpha" in Movie Recorder instead of logging an error when the encoder doesn't support an alpha channel.
- Changed the encoder API to improve the readability of scripted Recorders and allow users to use the ProRes encoder from script.

### Fixed
- Prevent the user from starting a recording session from the Recorder window if the Unity Editor has compiler errors.
- Make sure to always restore the asynchronous shader compilation setting's original value when the recording session ends.

## [4.0.0-pre.2] - 2021-09-14
### Added
- Added variable frame rate support to the Movie Recorder.

### Changed
- Make VP8 WebM the default output format for Movie Recorders on Linux, MP4 not being supported.
- Log errors when setting up a Recorder for H.264 MP4 or ProRes on Linux.
- Hide the Flip Vertical property in AOV Recorder Settings, as it is not supported.
- Disable asynchronous shader compilation before recording.
- The Exit Play Mode option, which used to be a per-user setting, is now a per-Recorder-List asset.

### Fixed
- Prevent recording when multiple Recorders use the same output file name.
- Ensure recording the expected frames with accumulation when the recording range doesn't start at 0.
- Add missing output location browse button in the Animation Clip Recorder.
- Ignore the unsupported "Capture UI" option in Scriptable Render Pipeline (SRP) projects to prevent recordings from being corrupted
- Prevent users from editing Recorder bindings through the UI.
- Add contextual feedback about Simulator view not supported when selecting "Game View" as the recorded source.
- Automatically switch to Game view if Simulator view is selected when starting to record the "Game View" source.
- Prevent the Recorder Timeline integration from outputting files when the timeline is paused.
- Ignore the Render Frame Step property when the Frame Rate Playback mode is set to Constant.
- Fix valid Recorders only recording the first frame when the first Recorder in the Recorder Window list is invalid.
- Prevent users from getting issues when renaming recorders in the Recorder List.

## [4.0.0-pre.1] - 2021-08-25
### Added
- Added a slider to change the JPEG quality when using Image Recorders or AOV Recorders.
- Added a new <AOV> wildcard for AOV Image Sequence recorders, and include it in the default file name.

### Changed
- Modified the Recorder menu item to cycle the Recorder Window status between open, in focus, and closed.
- Added <Recorder> wildcard to the default file name of all recorders.
- Moved the Recorder Options from Recorder menu items to Unity Editor user Preferences.
- Expose the AudioInput, AudioRecorderSettings, and WAVEncoder classes to the public API.

### Fixed
- Fix the "Open output location" button functionality in Linux.

## [3.0.1] - 2021-07-22
### Fixed
- Perform the appropriate color space conversion for Texture Sampling sources when required.
- Fix vertically flipped outputs on OpenGL hardware.

## [3.0.0] - 2021-06-17
### Changed
- Prevent invalid GPU callback data from being written to a frame: this change skips the problematic frame and logs an error message.

### Fixed
- Do not perform the color space conversion from linear to sRGB for RenderTextures that are already sRGB.
- Ensure that the color space conversion from sRGB to linear is performed when required for EXR files
- Fixed an exception that occurred when sending a RenderTexture to a Recorder before creating this RenderTexture.
- Fixed issues with the Recorder samples about synchronizing multiple recordings and resetting the Game view resolution.

## [3.0.0-pre.2] - 2021-05-17
### Added
- Added support for arm64 macOS.

### Fixed
- (macOS) Fixed an image stride issue for ProRes formats 4444 and 4444XQ.
- Fixed audio recording issue when the frame interval is not starting at 0.
- Fixed image artifacts on the first recorded frame with HDRP and TAA enabled.

## [3.0.0-pre.1] - 2021-04-07
### Added
- Added public API for AOVRecorderSettings.

### Changed
- Enabled alpha channel capture in projects that use HDRP.
- Changed the package display name from "Unity Recorder" to "Recorder" in the package manager.

### Fixed
- Fixed clipped text in the file path drop down.
- Fixed an exception that occurred when the user performed the undo action after deleting a Recorder.
- Fixed a wrong label for the WebM codec.
- Fixed invalid values in the alpha channel when performing texture sampling for different rendering and output resolutions.
- Ensure that the Image Recorder encodes in sRGB when requested, even if the scripted render pipeline provides linear data.
- Fixed a memory leak in the AOV Recorder.
- Fixed the Tagged Camera capture process to follow any camera changes that might occur.
- Improve the console messages for errors and warnings when Recorders are not properly configured.

### Removed
- Removed legacy Recorders: MP4, EXR, PNG, WEBM and GIF Animation.

## [2.6.0-exp.4] - 2021-02-22
### Fixed
- Fixed an "invalid AOV" error that occurred when selecting the Albedo AOV.

## [2.6.0-exp.3] - 2021-02-19
### Added
- Added support for recording accumulation in HDRP, for motion blur and path tracer.
- Integrated the AOV Recorder into this package.
  <br />**Note:** If you previously installed the formerly separate AOV Recorder package, you should uninstall it to avoid any unexpected recording issues.

### Fixed
- Fixed an issue where the Recorder Window's display name would be incorrect when using the Recorder Editor Sample.
- Fixed a UI issue where foldouts in the RecorderSettingPreset asset would not open when clicking their header name.

## [2.5.5] - 2021-02-26
### Fixed
- Fixed an error that occurred when setting the build target to macOS standalone from the Editor in Windows.

## [2.5.4] - 2021-01-25
### Fixed
- Correctly update the absolute path when choosing a path with the folder picker in the Recorder Window.
- Removed a console error message displayed when the Recorder cannot find a custom Recorder icon.
- Fixed an exception that would occur when Recording with Unity 2021.2.
- Fixed a rounding error in the delta time when recording with non-integer frame rates.

## [2.5.2] - 2020-12-16
### Fixed
- Fixed an issue where changing the active camera would be recorded one frame too late.
- Fixed an exception that would occur when undoing a Timeline Recorder Clip copy-paste action while the Inspector is active.
- Made the Animation Clip Recorder respect the Start Frame/Time setting instead of always recording from the first frame.
- Fixed Game View recording to get the expected rendering resolution, regardless of the current Game View dimensions.
- Fixed missing descriptions in Scripting API documentation.

## [2.5.0-pre.1] - 2020-11-02
### Added
- Added a new sample showing how to set up a movie recording session via script.

### Changed
- Removed support for versions prior to 2019.4.

### Fixed
- Fixed some errors with paths and wildcards in the sample code.
- Fixed broken internal links and inconsistencies in the user manual.

## [2.4.0-preview.1] - 2020-10-21
### Features
- Added MonoBehaviour recording support in the Animation Recorder.

### Bugfixes
- Fixed a bug where the animation recorder settings game object bindings were not saved properly when saving as a recorder preset.
- Reset the window without having to close it when a version upgrade happens while the Recorder Window is open.
- Fixed a bug where switching from an absolute path to any other path type would create an invalid output path.
- Log a warning when multiple concurrent Movie Recorder instances are concurrent, because this is not supported.
- Fixed a visual glitch with the "Cap FPS" checkbox extending outside its GUI element.
- Forced the "Render Frame Step" values in Recorders to be larger than zero.
- Fixed a bug causing excessive Timeline updates while changing the output file.
- Removed superfluous "CaptureAudio" option from the AudioRecorder.
- Fixed a bug that caused Copy/Pasted RecorderClips to lose the settings when entering in playmode.
- Fixed a visual glitch with very long paths when inspecting RecorderClips.
- Fixed a bug where in a Scriptable Render Pipeline, the GameView recordings would export transparency for PNG files if the camera background had transparency.
- Fixed a crash issue when starting and stopping a GIF animation recording while the Play Mode is paused.
- Fixed an issue where multiple recorders with various camera targets would produce flipped content.

## [2.3.0-preview.3] - 2020-09-23
### Features
- Added animation curve data compression setting with keyframe reduction options in the Animation Recorder.
- Added an option to set the generated animation key tangents to ClampedAuto in the Animation Recorder.
- Added ProRes encoder support in the MovieRecorder for OSX and Windows.
- Added option to specify the color space of the recorded images as either sRGB or Linear sRGB (unclamped).
- UX improvements

### Bugfixes
- Fixed a bug that in the texture sampler recorder for animated camera fov and animated physical properties
- Fixed a bug that caused the recorder to hang when docked next to the GameView.
- Fixed a bug that caused the recorder to hang if starting recording while already in playmode
- Fixed a bug where the AudioRecorder would not close the handle to the recorded file.
- Fixed a bug where the recorder could output multiple files in a single session, when only one was expected.
- Fixed a bug where one could not set a framerate larger than 120FPS
- Fixed a bug where the AnimationRecorder would not flush the data to disk when done recording.
- Fixed a bug where the RecorderWindow would get corrupted if some custom RecorderSettings code would not compile.
- Fixed a bug where the RecorderWindow would generate errors after a failed QuickRecord session.
- Fixed a bug where file creation could fail because of certain invalid characters in the file name.
- Fixed a bug where scripted Recorder sessions could generate errors if frames were recorded before the session was prepared.

### Known issues
- When the animationRecorder starts at frame zero and the timelineWindow inspects the timeline, multiple clip files are generated (some empty). To Fix, please update to the latest version of the Timeline package.

## [2.2.0-preview.4] - 2020-04-08
### Public API and bugfixes
- Added public api's to allow loading previously saved recorder lists.
- Fixed bug where the first few frames after going in play-mode were not recorded.
- Fixed a bug where the texture sampling recorder that did not correctly support the physical cameras.
- Fixed an issue when starting a record session from scripts.
- Fixed movie recoder's default framerate value being not set in the API. Default value is now set at 30 FPS.

## [2.1.0-preview.1] - 2019-12-17
### Public API and HDR recording
- Added public APIs to be able to implement custom recorders.
- Added the ability to enable recording HDR images (available only with HDRP)

## [2.0.3-preview.1] - 2019-08-13
### 2019.3, HDRP and build Fixes
- iOS Cloud build fails with an `exportArchive: Code signing "fccore.bundle" failed` error
- Recorder Clip with a Targeted Camera input gives black images with HDRP
- In 2019.3, HDRP is out of experimental and namespace was renamed
- Recorder settings warnings were not displayed in the console at recording start
- Massive documentation update
- Fixed "NullReferenceException" errors related to changes in GameView in 2019.3.0b1.

## [2.0.2-preview.2] - 2019-05-28
### Fixes and Linux support
- Add Gif and Legacy Recorders core library binaries for Linux
- Fix build errors related to fccore
- Fix a bug with Recorder Clip that would produce one frame movie if never displayed in the inspector
- Warning clean-up in 2018.4
- Remove ActiveCamera target when HDRP is available
- Fixed "NullReferenceException" errors related to changes in GameView in 2019.3

## [2.0.1-preview.1] - 2019-05-20
### Audio Recorder and some bug fixes
- Integrate AudioRecorder in the package
- Fixed Recorder labels that were not editable anymore
- Fixed Recorder List presets that were reloaded with an empty name
- Clean UIElements warning in 2019.2

## [2.0.0-preview.6] - 2019-03-26
### First package release of the *Unity Recorder*.
This mainly address moving from Asset Store to Package Manager. It also includes :
- Timeline dependency fix : since 2019.1 Timeline is a package. Code changes are compatible with
both 2018.3 (2018.3.4f1 and up) and 2019.1 (2019.1.0b2 and up).
- Updates to use official UIElements module as experimental API is deprecated in 2019.1.
- Warnings clean-up
- Samples fixes : documentation updates and proper asmdef to avoid issues during build
- Improved texture readback. Most speed improvements will be effective with previous versions,
 BUT at their top in 2019.1.
- Ability to capture a Light Weight Render Pipeline camera (requires Scriptable Render Pipeline > 5.3.0)

## [1.0.2] - 2018-09-07
### Custom resolutions. Multi-Scene editing. Various bug fixes.
- Ability to use custom resolution and custom aspect ratio
- Fixed GameObject reference sometimes resetting to None when in multi-scenes editing
- Fixed the Recorder Clip duplication issue
- Little improvement for errors reporting in the Recorder Window
- Fixed 360 View being too dark when in linear color space
- Fixed Flip Vertical being too dark in linear color space
- Fixed frame skipped issue when two or more recorders end at the same frame.
- Ability to change/reset the Take value for all recorders
- Added option to exit Playmode automatically when recording's stopped

## [1.0.1] - 2018-08-17
### 2018.1 support. Various UX fixes.
- Added support for 2018.1 (2018.1.9f1 and up)
- UI Fixes when reducing RecorderWindow size in 2018.2 and up
- Added icons for messages in the Recorder Status Bar
- Ability to use Arrow keys to switch between recorders
- Added visual indication for when the Recorder List has focus

## [1.0.0] - 2018-08-02
### First release of the *Unity Recorder*.
This is mainly a UX revamp of the Asset Store's Media Recorder. Main improvement is the ability to have multiple recorders in parallel.
