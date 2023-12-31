using System.Windows;
using System.Windows.Controls;

namespace AI_neural.UI;

public partial class SampleListItem
{
    private readonly ListView _owner;

    private string _itemName = null!;
    public BitImage8 Image;
    public bool AddedToHebb { get; private set; }

    public SampleListItem(ListView owner, string itemName, BitImage8 image)
    {
        _owner = owner;
        Image = image;
        InitializeComponent();
        SetName(itemName);
    }

    private void SetName(string name)
    {
        _itemName = name;
        Text.Text = name;
    }

    private void OnRemove(object sender, RoutedEventArgs e)
    {
        _owner.Items.Remove(this);
        SampleFile.Delete(_itemName);
    }

    private void OnRename(object sender, RoutedEventArgs e)
    {
        var name = SampleNameDialog.Prompt(_itemName);
        if (name == null) return;
        SetName(name);
    }

    public void AddToHebb()
    {
        AddedToHebb = true;
        Checked.Visibility = Visibility.Visible;
    }
}