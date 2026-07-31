# Troubleshooting

[!INCLUDE [banner-message](snippets/banner-message.md)]

The Multiplayer Widgets package has the following known issues:

* The Input System `com.unity.inputsystem@1.11.0` throws an exception when you use the dropdown of the **Select Input Audio Device** and **Select Output Audio Device** multiplayer widget. To fix this issue, downgrade the Input System package to `<= 1.10.0`. This issue will be fixed in the next release of `com.unity.inputsystem`.