﻿using System;// Alexa Davis CPT 206
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace theUnitedStatesOfAlexa
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.Load += new System.EventHandler(this.Form1_Load);
        }

        private void Form1_Load(object sender, EventArgs e)// Load Data into data grid
        {
            LoadStatesIntoComboBox();
            LoadStatesIntoDataGridView();
            this.Load += new System.EventHandler(this.Form1_Load);
            this.stateDrop.SelectedIndexChanged += new System.EventHandler(this.stateDrop_SelectedIndexChanged);
        }

        private void LoadStatesIntoComboBox()
        {
            try
            {
                using (var db = new DataClasses1DataContext())
                {
                    var stateNames = db.States.OrderBy(state => state.Name).Select(state => state.Name).ToList();
                    stateDrop.DataSource = stateNames;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load states into ComboBox: {ex.Message}");
            }
        }

        private void LoadStatesIntoDataGridView()
        {
            try
            {
                using (var db = new DataClasses1DataContext())
                {
                    // Fetch all states and load them into the DataGridView
                    var states = db.States.OrderBy(state => state.Name).ToList();
                    statesDataGridView.DataSource = states;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load states into DataGridView: {ex.Message}");
            }
        }


        private void InsertState()
        {
            using (var db = new DataClasses1DataContext())
            {
                
                State newState = new State
                {
                    Name = textBoxName.Text,
                    StateBird = textBoxBird.Text,
                    LargestCities = textBoxLargeCities.Text,
                    Capitol = textBoxCapitol.Text,
                    Colors = textBoxColors.Text,
                    FlagDescription = textBoxFlag.Text,
                    StateFlower = textBoxFlower.Text,
                    MedianIncome = decimal.Parse(textBoxMedIncome.Text), 
                    Population = int.Parse(textBoxPopulation.Text), 
                    ComputerJobsPercentage = double.Parse(textBoxCompJobs.Text) 
                };

                // Add the new object to the States table
                db.States.InsertOnSubmit(newState);

                try
                {
                    // Submit the changes to the database
                    db.SubmitChanges();
                    MessageBox.Show("State inserted successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to insert state: {ex.Message}");
                }
            }
        }
        private void buttonInsert_Click(object sender, EventArgs e)
        {
            InsertState();

        }

        private void stateDrop_SelectedIndexChanged(object sender, EventArgs e)
        {
           
            if (stateDrop.SelectedItem != null)
            {
                try
                {
                    var selectedStateName = stateDrop.SelectedItem.ToString();

                    using (var db = new DataClasses1DataContext())
                    {
                        // Query for information of the selected state
                        var selectedStateInfo = db.States
                                                  .Where(state => state.Name == selectedStateName)
                                                  .ToList(); // Assuming state names are unique

                       
                        statesDataGridView.DataSource = selectedStateInfo;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load information for the selected state: {ex.Message}");
                }
            }
        }

        private void buttonPrintState_Click(object sender, EventArgs e)
        {
            if (stateDrop.SelectedItem == null)
            {
                MessageBox.Show("Please select a state first.");
                return;
            }

            var selectedStateName = stateDrop.SelectedItem.ToString();
            try
            {
                using (var db = new DataClasses1DataContext())
                {
                    var selectedState = db.States.FirstOrDefault(s => s.Name == selectedStateName);
                    if (selectedState != null)
                    {
                        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        string htmlFilePath = Path.Combine(desktopPath, "SelectedStateInfo.html");

                        StringBuilder htmlContent = new StringBuilder();
                        htmlContent.AppendLine("<html><head><title>State Information</title></head><body>");
                        htmlContent.AppendLine("<h1>Selected State Information</h1>");
                        htmlContent.AppendLine($"<p>Name: {selectedState.Name}</p>");
                        htmlContent.AppendLine($"<p>Population: {selectedState.Population}</p>");
                        htmlContent.AppendLine($"<p>Flag Description: {selectedState.FlagDescription}</p>");
                        htmlContent.AppendLine($"<p>State Flower: {selectedState.StateFlower}</p>");
                        htmlContent.AppendLine($"<p>State Bird: {selectedState.StateBird}</p>");
                        htmlContent.AppendLine($"<p>Colors: {selectedState.Colors}</p>");
                        htmlContent.AppendLine($"<p>Largest Cities: {selectedState.LargestCities}</p>");
                        htmlContent.AppendLine($"<p>Capitol: {selectedState.Capitol}</p>");
                        htmlContent.AppendLine($"<p>Median Income: {selectedState.MedianIncome}</p>");
                        htmlContent.AppendLine($"<p>Computer Jobs Percentage: {selectedState.ComputerJobsPercentage}</p>");
                        htmlContent.AppendLine("</body></html>");

                        File.WriteAllText(htmlFilePath, htmlContent.ToString());

                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(htmlFilePath) { UseShellExecute = true });

                        MessageBox.Show($"Information for {selectedStateName} has been saved to {htmlFilePath} and will open in your browser.");
                    }
                    else
                    {
                        MessageBox.Show("State not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to generate HTML for state information: {ex.Message}");
            }
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = textBoxSearch.Text.Trim();

            if (string.IsNullOrEmpty(searchTerm))
            {
                MessageBox.Show("Please enter a search term.");
                return;
            }

            try
            {
                using (var db = new DataClasses1DataContext())
                {
                    
                    var searchResults = db.States
                        .Where(state => state.Name.Contains(searchTerm) ||
                                        state.StateBird.Contains(searchTerm) ||
                                        state.Capitol.Contains(searchTerm))
                        .ToList();

                   
                    statesDataGridView.DataSource = searchResults;

                    if (searchResults.Count == 0)
                    {
                        MessageBox.Show("No entries found matching the search criteria.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to perform search: {ex.Message}");
            }
        }

        private void buttonSortPopulation_Click(object sender, EventArgs e)
        {
            SortStates("Population");
        }

        private void buttonSortbyIncome_Click(object sender, EventArgs e)
        {
            SortStates("MedianIncome");
        }

        private void buttonSortAlphabetically_Click(object sender, EventArgs e)
        {
            SortStates("Alphabetically");
        }
        private void SortStates(string sortBy)
        {
            try
            {
                using (var db = new DataClasses1DataContext())
                {
                    IQueryable<State> query;

                    switch (sortBy)
                    {
                        case "Population":
                            query = db.States.OrderBy(state => state.Population);
                            break;
                        case "MedianIncome":
                            query = db.States.OrderBy(state => state.MedianIncome);
                            break;
                        case "Alphabetically":
                            query = db.States.OrderBy(state => state.Name);
                            break;
                        default:
                            query = db.States;
                            break;
                    }

                    statesDataGridView.DataSource = query.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to sort states: {ex.Message}");
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            var selectedStateName = stateDrop.SelectedItem.ToString();

            if (string.IsNullOrEmpty(selectedStateName))
            {
                MessageBox.Show("Please select a state first.");
                return;
            }

            try
            {
                using (var db = new DataClasses1DataContext())
                {
                    var stateToUpdate = db.States.FirstOrDefault(state => state.Name == selectedStateName);
                    if (stateToUpdate != null)
                    {
                        
                        stateToUpdate.StateBird = textBoxBird.Text;
                        

                        // Check and update Population if provided and valid
                        if (!string.IsNullOrWhiteSpace(textBoxPopulation.Text) && int.TryParse(textBoxPopulation.Text, out int population))
                        {
                            stateToUpdate.Population = population;
                        }
                        else if (!string.IsNullOrWhiteSpace(textBoxPopulation.Text))
                        {
                            MessageBox.Show("Invalid population format.");
                            return; // Exit if population input is invalid
                        }
                        

                        db.SubmitChanges();
                        MessageBox.Show("State updated successfully!");

                        
                        LoadStatesIntoDataGridView();
                    }
                    else
                    {
                        MessageBox.Show("Selected state not found in the database.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update state: {ex.Message}");
            }
        }





    }
}
