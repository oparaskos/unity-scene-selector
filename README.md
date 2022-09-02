[![openupm](https://img.shields.io/npm/v/uk.me.paraskos.oliver.scene-selector?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/uk.me.paraskos.oliver.scene-selector/)

# Scene Selector

Select a list of scenes rather than using build indices directly.

<img width="660" alt="Screenshot 2022-09-02 at 14 05 19" src="https://user-images.githubusercontent.com/8076088/188152133-2d11039e-9c2d-4578-bfbe-99ecfba457b6.png">

If scenes are removed from the build then the drawer gives a warning.
<img width="661" alt="Screenshot 2022-09-02 at 14 05 47" src="https://user-images.githubusercontent.com/8076088/188152219-b6f50b90-cf15-4bfa-997a-8b42c4352d6c.png">

Scenes are serialised as asset GUIDs and build indexes which are synced regularly to preserve proper reference integrity if scene order in the build settings is changed.
