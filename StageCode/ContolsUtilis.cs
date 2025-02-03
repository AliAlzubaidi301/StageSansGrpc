using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace StageCode
{
    public static class ControlUtils
    {
        private static List<Variable> _visibleVariables = new List<Variable>();

        public static void RegisterControl(Control component, Func<string> getVisibility,
            Action<EventHandler> visibilityChangingHandler, Action<EventHandler> visibilityChangedHandler)
        {
            // Ajoute un gestionnaire à l'événement Paint du contrôle
            component.Paint += (sender, e) =>
            {
                try
                {
                    if (!IsVisible(getVisibility()))
                    {
                        DrawRedRectangle(component, e);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur lors du rendu : " + ex.Message);
                }
            };

            // Gestion de la visibilité en changement
            visibilityChangingHandler((sender, e) =>
            {
                string visibility = getVisibility();
                if (visibility.StartsWith("#"))
                {
                    UnregisterVariable(visibility);
                }
            });

            // Gestion de la visibilité après changement
            visibilityChangedHandler((sender, e) =>
            {
                string visibility = getVisibility();
                if (visibility.StartsWith("#"))
                {
                    RegisterVariable(visibility, component);
                }

                component.Refresh(); // Force le rafraîchissement du contrôle
            });
        }

        private static bool IsVisible(string visibility)
        {
            // Implémentation simplifiée de la logique de visibilité
            return visibility != "0";
        }

        // Dessine un rectangle rouge autour du contrôle et ajoute une croix diagonale
        private static void DrawRedRectangle(Control control, PaintEventArgs e)
        {
            using (Graphics graphics = e.Graphics)
            using (Pen pen = new Pen(Color.Red, 5f))
            {
                // Dessine un rectangle rouge
                graphics.DrawRectangle(pen, new Rectangle(new Point(0, 0), control.Size));

                // Réduit l'épaisseur pour les lignes diagonales
                pen.Width = 3f;

                // Dessine une croix
                graphics.DrawLine(pen, new Point(control.Width, 0), new Point(0, control.Height));
                graphics.DrawLine(pen, new Point(0, 0), new Point(control.Width, control.Height));
            }
        }

        // Enregistre une variable de visibilité et attache un rafraîchissement au composant
        private static Variable RegisterVariable(string variableName, Control component)
        {
            var variable = _visibleVariables.FirstOrDefault(v => string.Equals(v.VarName, variableName, StringComparison.OrdinalIgnoreCase));

            if (variable == null)
            {
                variable = new Variable
                {
                    VarName = variableName,
                    Visible = true
                };

                variable.VisibilityChanged += (sender, e) => component.Refresh();

                _visibleVariables.Add(variable);
            }

            variable.Count++;
            return variable;
        }

        // Désenregistre une variable de visibilité
        private static void UnregisterVariable(string variableName)
        {
            var variable = _visibleVariables.FirstOrDefault(v => string.Equals(v.VarName, variableName, StringComparison.OrdinalIgnoreCase));

            if (variable != null)
            {
                variable.Count--;

                if (variable.Count <= 0)
                {
                    _visibleVariables.Remove(variable);
                }
            }
        }
    }

    // Classe représentant une variable de visibilité
    public class Variable
    {
        public string VarName { get; set; }
        public bool Visible { get; set; }
        public int Count { get; set; } = 0;
        public event EventHandler VisibilityChanged;

        public void TriggerVisibilityChanged()
        {
            VisibilityChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
