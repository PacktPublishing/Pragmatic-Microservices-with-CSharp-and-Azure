namespace Codebreaker.WPF.Views.Components;

public partial class PegSelectionView : UserControl
{
    public PegSelectionView()
    {
        DataContext = this;
        InitializeComponent();
    }

    public GamePageViewModel ViewModel
    {
        get => (GamePageViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register("ViewModel", typeof(GamePageViewModel), typeof(PegSelectionView), new PropertyMetadata(null));
}
