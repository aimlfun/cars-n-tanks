using System.Reflection;

namespace CarsAndTanks.Utilities;

/// <summary>
/// General purpose utilities.
/// </summary>
internal static class Utils
{
    /// <summary>
    /// Removes every control from another control, and disposes of them. 
    /// </summary>
    /// <param name="control"></param>
    internal static void RemoveControls(Control control)
    {
        foreach (Control c in control.Controls)
        {
            control.Controls.Remove(c);

            if (c.Name != "panelTrack") // dispose of this, and the app will break
            {
                c.Dispose();
            }
        }

        control.Controls.Clear(); // just to be sure.
    }

    /// <summary>
    /// Returns a list of colors except for those that classh with road.
    /// </summary>
    /// <returns></returns>
    internal static List<Color> ColorStructToList()
    {
        List<Color> list = new(typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public)
                            .Select(c =>
                            {
                                var color = c.GetValue(null, null);
                                return color is null ? Color.Black : (Color)color;
                            })
                            .ToList());

        list.Remove(Color.Transparent); // transparent results in seeing under the main window, rather than track!
        list.Remove(Color.LightPink); // used as transparency in seeing under the main window, rather than track!
        list.Remove(Color.Gray); // conflict with road
        list.Remove(Color.DarkGray); // conflict with road

        return list;
    }

}
