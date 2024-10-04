using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PersonalDiary
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Window
    {

        public Home()
        {
            InitializeComponent();
        }

        // Add Entry Nav
        private void btnNavItem1_Click(object sender, RoutedEventArgs e)
        {
            btnAddEntry.Visibility = Visibility.Visible;
            TxtBoxNewEntry.Visibility = Visibility.Visible;
            txtDisplayEntries.Visibility = Visibility.Collapsed;
            TxtBoxDeleteId.Visibility = Visibility.Collapsed;
            btnDeleteEntry.Visibility = Visibility.Collapsed;
        }
        // Delete Entry Nav
        private void btnNavItem2_Click(object sender, RoutedEventArgs e)
        {
            btnAddEntry.Visibility = Visibility.Collapsed;
            TxtBoxNewEntry.Visibility = Visibility.Collapsed;
            txtDisplayEntries.Visibility = Visibility.Collapsed;

            // Show delete controls
            TxtBoxDeleteId.Visibility = Visibility.Visible;
            btnDeleteEntry.Visibility = Visibility.Visible;
        }
        // View Entry Nav
        private void btnNavItem3_Click(object sender, RoutedEventArgs e)
        {
            btnAddEntry.Visibility = Visibility.Collapsed;
            TxtBoxNewEntry.Visibility = Visibility.Collapsed;
            txtDisplayEntries.Visibility = Visibility.Visible;
            TxtBoxDeleteId.Visibility = Visibility.Collapsed;
            btnDeleteEntry.Visibility = Visibility.Collapsed;

            int getYear = DateTime.Now.Year;

            string connectionString = "Data Source=entries.db";
            using (var connection = new SqliteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT id, entry FROM entries";
                    using (var command = new SqliteCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        StringBuilder output = new StringBuilder();
                        HashSet<string> uniqueEntries = new HashSet<string>();
                        int count = 0;

                        while (reader.Read())
                        {
                            count++;
                            int id = reader.GetInt32(0);
                            string entry = reader.GetString(1);
                            string debugInfo = $"ID:{id},Entry: {entry}";

                            if (uniqueEntries.Add(debugInfo))
                            {
                                output.AppendLine(debugInfo);
                            }
                            //else
                            //{
                            //    output.AppendLine($"Duplicate found: {debugInfo}");
                            //}
                        }

                        output.Insert(0, $"{getYear} | Total entries: {count}\n\n");

                        if (output.Length > 0)
                        {
                            txtDisplayEntries.Text = output.ToString();
                        }
                        else
                        {
                            MessageBox.Show("No entries found.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                catch (SqliteException ex)
                {
                    MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAddEntry_Click(object sender, RoutedEventArgs e)
        {
            string database_connection = "Data Source=entries.db;";
            using (var connection = new SqliteConnection(database_connection))
            {
                try
                {
                    connection.Open();

                    // Create table if it doesn't exist
                    string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS entries (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    entry TEXT NOT NULL,
                    active INTEGER NOT NULL
                    )";
                    using (var createTableCommand = new SqliteCommand(createTableQuery, connection))
                    {
                        createTableCommand.ExecuteNonQuery();
                    }

                    // Insert new entry
                    if (string.IsNullOrWhiteSpace(TxtBoxNewEntry.Text))
                    {
                        MessageBox.Show("Entries must contain text.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        string insertQuery = "INSERT INTO entries (entry, active) VALUES (@Entry, @Active)";
                        using (var insertCommand = new SqliteCommand(insertQuery, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@Entry", TxtBoxNewEntry.Text);
                            insertCommand.Parameters.AddWithValue("@Active", 1);
                            insertCommand.ExecuteNonQuery();
                        }

                        MessageBox.Show("Entry added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        TxtBoxNewEntry.Clear();
                    }
                }

                catch (SqliteException ex)
                {
                    MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }
        private void btnDeleteEntry_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(TxtBoxDeleteId.Text, out int id))
            {
                MessageBox.Show("Please enter a valid ID number.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string database_connection = "Data Source=entries.db;";
            using (var connection = new SqliteConnection(database_connection))
            {
                try
                {
                    connection.Open();

                    string deleteQuery = "DELETE FROM entries WHERE id = @Id";
                    using (var deleteCommand = new SqliteCommand(deleteQuery, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@Id", id);
                        int rowsAffected = deleteCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Entry deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            TxtBoxDeleteId.Clear();
                        }
                        else
                        {
                            MessageBox.Show("No entry found with the specified ID.", "Not Found", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                catch (SqliteException ex)
                {
                    MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
