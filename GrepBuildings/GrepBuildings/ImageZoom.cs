using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GrepBuildings
{
    public partial class ImageZoom : Form
    {
        public ImageZoom()
        {
            InitializeComponent();
        }

        public void setLocation(int p_X, int p_Y)
        {
            this.Location = new Point(p_X, p_Y);
        }

        public void setImageByName(string p_Building)
        {
            switch (p_Building)
            {
                case "main":
                    this.BackgroundImage = global::GrepBuildings.Properties.Resources.main;
                    break;
                case "hide":
                    this.BackgroundImage = global::GrepBuildings.Properties.Resources.hide;
                    break;
                case "storage":
                    this.BackgroundImage = global::GrepBuildings.Properties.Resources.storage;
                    break;
                case "farm":
                    this.BackgroundImage = global::GrepBuildings.Properties.Resources.farm;
                    break;
                case "lumber":
                    this.BackgroundImage = global::GrepBuildings.Properties.Resources.lumber;
                    break;
                case "stoner":
                    this.BackgroundImage = global::GrepBuildings.Properties.Resources.stoner;
                    break;
                case "ironer":
                    this.BackgroundImage = global::GrepBuildings.Properties.Resources.ironer;
                    break;
                case "market":
                    this.BackgroundImage = global::GrepBuildings.Properties.Resources.market;
                    break;
                case "docks":
                    this.BackgroundImage = global::GrepBuildings.Properties.Resources.docks;
                    break;
                case "barracks":
                    this.BackgroundImage = global::GrepBuildings.Properties.Resources.barracks;
                    break;
                case "wall":
                    this.BackgroundImage = global::GrepBuildings.Properties.Resources.wall;
                    break;
                case "academy":
                    this.BackgroundImage = global::GrepBuildings.Properties.Resources.academy;
                    break;
                case "temple":
                    this.BackgroundImage = global::GrepBuildings.Properties.Resources.temple;
                    break;
                case "theater":
                    this.BackgroundImage = global::GrepBuildings.Properties.Resources.theater;
                    break;
                case "thermal":
                    this.BackgroundImage = global::GrepBuildings.Properties.Resources.thermal;
                    break;
                case "library":
                    this.BackgroundImage = global::GrepBuildings.Properties.Resources.library;
                    break;
                case "lighthouse":
                    this.BackgroundImage = global::GrepBuildings.Properties.Resources.lighthouse;
                    break;
                case "tower":
                    this.BackgroundImage = global::GrepBuildings.Properties.Resources.tower;
                    break;
                case "statue":
                    this.BackgroundImage = global::GrepBuildings.Properties.Resources.statue;
                    break;
                case "oracle":
                    this.BackgroundImage = global::GrepBuildings.Properties.Resources.oracle;
                    break;
                case "trade_office":
                    this.BackgroundImage = global::GrepBuildings.Properties.Resources.trade_office;
                    break;
            }
            this.Height = 40;
            this.Width = 40;
        }
    }
}
