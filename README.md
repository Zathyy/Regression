# Regression
 Used for regression issue to Slang-lang

Run the GenerateProjects.bat

Then a Generated folder will be generated containing the Solution

To see the behaviour that's happening go into the Debug folder and open the solution file
then open src/Regression.Sharpmake.cs and change the useSDK Variable inside the function ConfigureAll
then generate the project files every time changing it

Expected output with useSdk: Entry points: 2
Expected output without useSdk: Entry points: 0