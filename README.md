XML Parser Application Technical Documentation

Overview
The XML Parser application is a web application that takes XML files, converts them to JSON, and saves them locally. The application consists of two main components: an ASP.NET Web API backend and a React frontend.

Backend (ASP.NET Web API)
The backend is responsible for handling the conversion of XML files to JSON. It is implemented using ASP.NET Web API and runs on https://localhost:7217. The backend exposes a single endpoint, /document, which accepts an XML file as a multipart form-data request, converts it to JSON, stores it locally and returns JSON location in the response.

Frontend (React)
The frontend is a React application that provides the user interface for the application. It allows users to select an XML file, start the conversion process, and receive the location of the new JSON file.

Application Usage
To use the XML Parser application, follow these steps:

- navigate to \backend\XMLParserAPI folder
- start the ASP.NET Web API backend by running the following command:
dotnet run

- navigate to \frontend folder
- start the React frontend by running the following command:
npm start

- open the React frontend in your web browser at http://localhost:3000.

- select an XML file and click the "Submit" button.

The application will convert the XML file to JSON and display the location of the new JSON file.

Technical Specifications
The backend is implemented using ASP.NET Web API and runs on .NET 6.

The frontend is implemented using React 18.2.0.

The application uses the following libraries:

ASP.NET Web API
React
Chakra