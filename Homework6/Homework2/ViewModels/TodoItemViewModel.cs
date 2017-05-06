using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Homework2.ViewModels
{
    public class TodoItemViewModel
    {
        private ObservableCollection<Models.TodoItem> allItems = new ObservableCollection<Models.TodoItem>();
        public ObservableCollection<Models.TodoItem> AllItems { get { return this.allItems; } }

        private Models.TodoItem selectedItem = default(Models.TodoItem);
        public Models.TodoItem SelectedItem { get { return selectedItem; } set { this.selectedItem = value; } }

        public TodoItemViewModel()
        {
            // 加入两个用来测试的item
            //this.allItems.Add(new Models.TodoItem("作业1", "我的作业1"));
            //this.allItems.Add(new Models.TodoItem("作业2", "我的作业2"));

            var db = App.conn;
            using (var statement = db.Prepare(App.SQL_QUERY_VALUE)) {
                while (SQLiteResult.ROW == statement.Step()) {
                    //var i = new MessageDialog(statement[3].ToString()).ShowAsync();
                    this.allItems.Add(new Models.TodoItem(statement[0].ToString(),statement[1].ToString(),Convert.ToDateTime(statement[2].ToString()),Convert.ToBoolean(statement[3].ToString()),statement[4].ToString()));
                }
            }
        }

        public void AddTodoItem(string title, string description,DateTime date, String path)
        {
            this.allItems.Add(new Models.TodoItem(title, description,date,false,path));
        }

        public void RemoveTodoItem(string id)
        {
            // DIY
            this.allItems.Remove(this.selectedItem);
            // set selectedItem to null after remove
            this.selectedItem = null;
        }

        public void UpdateTodoItem(string id, string title, string description, DateTime date,String path)
        {
            // DIY
            this.selectedItem.title = title;
            this.selectedItem.description = description;
            this.selectedItem.date = date;
            this.selectedItem.path = path;
            // set selectedItem to null after update
            this.selectedItem = null;
        }

    }
}

