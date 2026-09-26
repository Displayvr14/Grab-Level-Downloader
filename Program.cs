using Raylib_cs;
using rlImGui_cs;
using ImGuiNET;
using System.Numerics;
using System.Linq.Expressions;

Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
Raylib.InitWindow(1200, 800, "Grab Level Downloader");
rlImGui.Setup(true);

var style = ImGui.GetStyle();
style.WindowRounding = 6.0f;
style.FrameRounding = 4.0f;
style.PopupRounding = 4.0f;

string link = "";
string outputDirectory = Directory.GetCurrentDirectory();

ADB.RunCommand("devices");

while (!Raylib.WindowShouldClose())
{
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.Black);
    rlImGui.Begin();

    if (ImGui.Begin("Level Downloader"))
    {
        if (ImGui.BeginTable("MainFormTable", 2, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.BordersOuter))
        {
            ImGui.TableSetupColumn("Labels", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("Inputs", ImGuiTableColumnFlags.WidthStretch);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding(); 
            ImGui.Text("Level Link");
            
            ImGui.TableSetColumnIndex(1);
            ImGui.SetNextItemWidth(-1); 
            ImGui.InputText("##LevelLinkInput", ref link, 100); 

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();
            ImGui.Text("Output Directory");
            
            ImGui.TableSetColumnIndex(1);
            ImGui.SetNextItemWidth(-1);
            ImGui.InputText("##OutputDirInput", ref outputDirectory, 100);

            ImGui.EndTable();
        }

        ImGui.Spacing();
        
        if (ImGui.Button("Download", new Vector2(-1, 30)))
        {
            _ = Task.Run(async () =>
            {
                try 
                { 
                    await GrabApi.DownloadLevelAsync(
                        GrabApi.getLevelDataWithLink(link)[0],
                        GrabApi.getLevelDataWithLink(link)[1], 
                        outputDirectory
                        ); 
                } 
                catch (Exception ex) 
                { 
                    GrabApi.logs.Add($"[ERROR]: {ex}"); 
                }
            });
        }
        if (ImGui.Button("Download To Headset (ADB)", new Vector2(-1, 30)))
        {
            _ = Task.Run(async () =>
            {
                try 
                { 
                    string fileDir = await GrabApi.DownloadLevelAsync(
                        GrabApi.getLevelDataWithLink(link)[0],
                        GrabApi.getLevelDataWithLink(link)[1], 
                        outputDirectory
                        ); 
                    if (ADB.HasConnectedDevices())
                    {
                        ADB.RunCommand($"push {fileDir} /sdcard/android/data/com.slindev.grab_demo/files/levels/user");
                        GrabApi.logs.Add("Downloaded level to headset");
                    }
                    else
                    {
                        GrabApi.logs.Add("[ERROR]: No connected headsets");
                    }
                    File.Delete(fileDir);
                } 
                catch (Exception ex) 
                { 
                    GrabApi.logs.Add($"[ERROR]: {ex}"); 
                }
            });
        }
        ImGui.End();
    }

    if (ImGui.Begin("Logs"))
    {
        if (ImGui.BeginChild("LogScrollRegion", new Vector2(0, 0), ImGuiChildFlags.None, ImGuiWindowFlags.HorizontalScrollbar))
        {
            foreach (var logLine in GrabApi.logs.ToArray())
            {
                if (logLine.StartsWith("[ERROR]"))
                {
                    ImGui.TextColored(new Vector4(1.0f, 0.4f, 0.4f, 1.0f), logLine);
                }
                else
                {
                    ImGui.TextColored(new Vector4(0.4f, 1.0f, 0.4f, 1.0f), logLine); 
                }
            }
            if (ImGui.GetScrollY() >= ImGui.GetScrollMaxY())
            {
                ImGui.SetScrollHereY(1.0f);
            }
            ImGui.EndChild();
        }
        ImGui.End();
    }

    rlImGui.End();
    Raylib.EndDrawing();
}

rlImGui.Shutdown();
Raylib.CloseWindow();
