   M S B A M L   `   `   `  ���� B  >WpfApp1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:MyServerMyServer      MyServer.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     (localclr-namespace:MyServer     a ]Awesomium.Windows.Controls, Version=1.6.6.0, Culture=neutral, PublicKeyToken=7a34e179b8b61c39/Custom"http://schemas.awesomium.com/winfx    � Title$  	四维度��$	��600��$	��800��
�������$��60��$��60��$��0q� � ShowInTaskbar .  .��� .+��� -    ��Name$ TextBox_Data��$
��Left=�$��10,4,0,0q� �� TextWrapping$
 Wrap=�$	��Top=����	#FF020101$	��200��$	��566�����	#FFD7D4C9�� -   $��扫描串口��$
��Left=�$��
588,75,0,0q�$	��Top=�$��90��$��18(��� -   $��ComboBox_PortNumber��$��18(�$
��Left=�$��	671,5,0,0q�$	��Top=�$	��111��$��30���� $��	串口号��$
��Left=�$��	588,5,0,0q�$	��Top=�$��30��$��75��$��18(��� -   $��ComboBox_BaudRate��$
��Left=�$��
671,40,0,0q�$	��Top=�$	��111��$��30��$��18(�.@��� $	��300��$
��Left=�$	��109���� $	��600��$
��Left=�$	��109���� $
��4800��$
��Left=�$	��109���� $
��9600��$
��Left=�$	��109���� $��14400��$
��Left=�$	��109���� $��115200��$
��Left=�$	��109���� $��	波特率��$
��Left=�$��
588,40,0,0q�$	��Top=�$��30��$��75��$��18(��� -   $��清空数据��$
��Left=�$��588,142,0,0q�$	��Top=�$��90��$��18(��� -   $��保留数据��$
��Left=�$��689,142,0,0q�$	��Top=�$��93��$��18(��� -   $��18(�$
��Left=�$��689,107,0,0q�$	��Top=�$��93��$��30��.@��� $	��Hex��$
��Left=�$	��109���� $
��Text��$
��Left=�$	��109���� $��数据格式��$
��Left=�$��588,107,0,0q�$	��Top=�$��30��$��90��$��18(��� -    ��Name$ ButtonOpenPort��$��打开串口��$
��Left=�$��
689,75,0,0q�$	��Top=�$��93��$��18(��� $��已经接收数据：��$
��Left=�$��
10,204,0,0q�$	��Top=�$��30��$	��125��$��18(��� $��已经发送数据：��$
��Left=�$��346,204,0,0q�$	��Top=�$��30��$	��125��$��18(��� -	    ��Name$ Lable_ReadDataNumber��$��0��$
��Left=�$��135,204,0,0q�$	��Top=�$��30��$	��110��$��18(��� -
   $ Lable_SendDataNumber��$��0��$
��Left=�$��466,204,0,0q�$	��Top=�$��30��$	��110��$��18(�