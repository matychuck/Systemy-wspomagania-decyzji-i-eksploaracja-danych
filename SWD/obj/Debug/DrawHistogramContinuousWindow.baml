   M S B A M L   `   `   `  ����>  :SWD, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:SWDSWD  5   	   '    !SWD.DrawHistogramContinuousWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   #localclr-namespace:SWD     5   	   +oxyhttp://oxyplot.codeplex.com     5   	   R NOxyPlot.Wpf, Version=2.0.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0!oxy1http://oxyplot.org/wpf  5	   	     � Title$   Histogram zmienna ciągła��5
   	   $	��450��6*   $	��800��67   .��5      � .+�5   
     OxyPlot.Wpf.PlotView  -   5        $
��plot��5   2      Model 6   �� *	MyModel+62   �� 5   
   -   5         ��Name$ comboBox��5      $
��Left=�6%   $��
145,14,0,0q�6@   $	��Top=�6T   $	��120��6l   6x   �� 5   
   -   5         ��Name$ label��5      $��Wybierz kolumnę��6   $
��Left=�69   $��	30,10,0,0q�6T   $	��Top=�6g    J� RenderTransformOrigin$ 
0.49,0.577&�6   �� 5   
   -   5        $ label1��5      $��Przedziały��6    $
��Left=�65   $��
635,10,0,0q�6P   $	��Top=�6d   �� 5   
   -   5         ��Name$ 
partitions��5      $
��Left=�6&   $��23��6A   $��
703,13,0,0q�6M    �� TextWrapping$
 Wrap=�6a   $�� ��6u   $	��Top=�6}   $��79��6�   5      5      