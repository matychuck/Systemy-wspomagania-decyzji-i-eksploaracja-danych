   M S B A M L   `   `   `  ����>  :SWD, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:SWDSWD  5   	       SWD.ScatterPlotWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   #localclr-namespace:SWD     5   	   +oxyhttp://oxyplot.codeplex.com     5   	   R NOxyPlot.Wpf, Version=2.0.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0!oxy1http://oxyplot.org/wpf  5   	     � Title$  ScatterPlotWindow��5
   	   $	��550��6#   $	��900��60   .��5      � .+�5   
     OxyPlot.Wpf.PlotView  -   5        $
��plot��5   2   $��10,55,132,0q�6>      Model 6   �� *	MyModel+6>   �� 5   
   -   5         ��Name$ 	comboBoxX��5      $
��Left=�6&   $��
101,14,0,0q�6A   $	��Top=�6U   $	��120��6m   �� 5   
   -   5         ��Name$ label��5      $��	Zmienna X��6   $
��Left=�63   $��	20,10,0,0q�6N   $	��Top=�6a    J� RenderTransformOrigin$ 
0.49,0.577&�6y   �� 5   
   -   5        $ 	comboBoxY��5      $
��Left=�6&   $��
311,14,0,0q�6A   $	��Top=�6U   $	��120��6m   �� 5   
   -   5        $ label2��5      $��	Zmienna Y��6    $
��Left=�64   $��
240,10,0,0q�6O   $	��Top=�6c   $ 
0.49,0.577&�6{   �� 5   
   -   5        $ 	comboBoxC��5      $
��Left=�6&   $��
502,14,0,0q�6A   $	��Top=�6U   $	��120��6m   �� 5   
   -   5        $ label3��5      $��Klasa��6    $
��Left=�60   $��
460,10,0,0q�6K   $	��Top=�6_   $ 
0.49,0.577&�6w   �� 5   
   -   5         ��Name$ 
drawButton��5      $��Rysuj��6%   $
��Left=�65   $��
685,14,0,0q�6P   $	��Top=�6d   $��75��6|   6�   � 5   
   -	   5        $��
legendGrid��5   o   $
��Left=�6   $	��454��6*   $��
765,55,0,0q�67   $	��Top=�6K   $	��117��6c   $ 0.385,0.374&�6�   5      �� 5   
   -
   5         ��Name$ theSameColors��5      $��No colors change��6*   $
��Left=�6E   $��
775,19,0,0q�6`   $	��Top=�6t   5      5      