   M S B A M L   `   `   `  ����>  :SWD, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:SWDSWD  5   	       SWD.KAverageWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   #localclr-namespace:SWD     5   	     � Title$  KAverageWindow��5   	   $	��640��6    $	��600��6-   .��5	      � .+�5
   
   �� -   5         ��Name$ classifyButton��5
      $��
Klasyfikuj��6)   $
��Left=�6>   $��
320,27,0,0q�6Y   $	��Top=�6m   $��75��6�   6�   �� 5   
   -   5         ��Name$ metricsComboBox��5      $
��Left=�6,   $��
175,27,0,0q�6G   $	��Top=�6[   $	��120��6s   �� 5   
   -   5         ��Name$ kNumber��5      $
��Left=�6#   $��23��6>   $��	20,27,0,0q�6J    �� TextWrapping$
 Wrap=�6]   $�� ��6q   $	��Top=�6y   $	��120��6�   &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ dataGridView2��5      $	��495��6+   $��	20,90,0,0q�68   $	��Top=�6K   $	��540��6c      CanUserAddRows .     AutoGenerateColumns . $
��Left=�5   6   $��Stretch=�6Q      ColumnWidth5  /System.Windows.Controls.DataGridLengthConverter$ * 6v   	   SelectionMode$	 Extended=�5   3   
   SelectionUnit$
 FullRow=�6L      
IsReadOnly .  ��6   �� 6d   �� 5   
   -   5         ��Name$ label��5      $��Liczba k��6   $
��Left=�62   $��15,1,0,0q�6M   $	��Top=�6_   �� 5   
   -   5        $ label1��5      $��Metryka��6    $
��Left=�62   $��	170,1,0,0q�6M   $	��Top=�6`   �� 5   
   -   5        $ label2��5      $��Zestawienie klastrów��6    $
��Left=�6?   $��	15,64,0,0q�6Z   $	��Top=�6m   �� 5   
   -   5        $ mergeButton��5      $��Merge��6&   $
��Left=�66   $��
407,27,0,0q�6Q   $	��Top=�6e   $��75��6}   6�   �� 5   
   -	   5         ��Name$ checkBox��5      $��Miernik 30% błędnych��6%   $
��Left=�6D   $��
320,64,0,0q�6_   $	��Top=�6s   5      5      