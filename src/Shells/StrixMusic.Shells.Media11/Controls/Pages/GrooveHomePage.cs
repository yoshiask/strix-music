using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Media11.Controls.Pages
{
    /// <summary>
    /// A <see cref="Control"/> to display a <see cref="LibraryViewModel"/> on a page.
    /// </summary>
    public partial class Media11HomePage : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Media11HomePage"/> class.
        /// </summary>
        public Media11HomePage()
        {
            DefaultStyleKey = typeof(Media11HomePage);
        }

        /// <summary>
        /// The backing property for <see cref="Library"/>.
        /// </summary>
        public static readonly DependencyProperty LibraryProperty =
            DependencyProperty.Register(nameof(Library), typeof(LibraryViewModel), typeof(Media11HomePage), new PropertyMetadata(null, (d, e) => ((Media11HomePage)d).OnLibraryChanged()));

        /// <summary>
        /// The library displayed in this view.
        /// </summary>
        public LibraryViewModel? Library
        {
            get { return (LibraryViewModel)GetValue(LibraryProperty); }
            set { SetValue(LibraryProperty, value); }
        }

        private void OnLibraryChanged()
        {
        }
    }
}
