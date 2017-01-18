using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IR_Engine
{
    interface ISearcher : INotifyPropertyChanged
    {
        string DocResult { set; get; }
        void move(double speed, int angle);

        bool proccessQuery(string query);
      //  void startSearcher();

        void initiate();
      
    }
}
