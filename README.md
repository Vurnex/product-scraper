# Product Scraper

A project / app to scrape product info from two target websites. (Newegg and Ebay). It was created using Windows Forms and WCF Library on Visual Studio 2019.

## How It Works

There is a Client side and Server side. The client side is a windows forms app and the server side is ran through a WCF Library.

On the client side, the user can choose the type of product and brand they would like to gather information on. Upon clicking the 'Get Data' button, the server will query the data currently stored in the database based on the inputs provided. The data will be displayed in each textbox in JSON formatting.

When clicking the 'Refresh Data' button, the server will scrape the data from the websites and store it in the database under each websites' table.

The intended function of the Generate Report button was to take the data and convert it to an HTML file that can be displayed in the browser. 

The server makes use of HTML Agility Pack to scrape data from the websites.

## What I Learned While Working On This Project

- Working with JSON data
- Scraping websites using HTML Agility Pack
- Using WCF Service Library
- Accessing and storing data in an AWS Database

*Most of the project files have been omitted to reduce the size and time to upload. Only the code demonstrating the implementation is left.

## Pictures

Main 

![image](https://user-images.githubusercontent.com/107071736/210182888-3a8e4561-1dd1-4920-ae68-c2790389f878.png)

Data Displayed

![image](https://user-images.githubusercontent.com/107071736/210182940-ae4fccb4-1e43-4f61-aea7-3a3319809a99.png)

