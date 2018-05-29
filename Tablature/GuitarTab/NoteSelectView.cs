using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GuitarTab
{
    public class NoteSelectView : BaseViewModel
    {
        private Visibility visibility;
        public Visibility Visibility
        {
            get { return visibility; }
            set { SetProperty(ref visibility, value); }
        }

        private NodeClick initial_click;
        private ContinueNoteSelectDelegate continue_command;

        public NoteSelectView()
        {
            Visibility = Visibility.Collapsed;
            initial_click = null;
            continue_command = null;
        }

        public void launchNoteSelect(NoteSelectLaunchEventArgs args)
        {
            initial_click = args.Click;
            continue_command = args.Command;
            Visibility = Visibility.Visible;
        }

        public void noteSelected(NodeClick click)
        {
            if (click.NoteNodes.Any())
            {
                NodeClick combined = createCombinedClick(click);
                continue_command?.Invoke(combined);
            }

            Visibility = Visibility.Collapsed;
            initial_click = null;
            continue_command = null;
        }

        public NodeClick createCombinedClick(NodeClick select_click)
        {
            NoteTreeNode init = initial_click.NoteNodes.First();
            NoteTreeNode select = select_click.NoteNodes.First();
            if (init.getNote().Position.occursBefore(select.getNote().Position))
            {
                initial_click.NoteNodes.Add(select);
                return initial_click;
            }
            else
            {
                select_click.NoteNodes.Add(init);
                return select_click;
            }
        }
    }
}
