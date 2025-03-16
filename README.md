[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/6tAGuDvy)
# Assignment 3: Working with Razor Components

In our last class, you created a Blazor Server application and simulated user growth. This class, we will introduce **Razor components**: how to build them, link them, and apply CSS to improve the user interface.

By the end of this class, you should be able to:

1. Understand the fundamentals of Razor components.  
2. Build and link multiple Razor components.  
3. Apply basic CSS to style your application.

In this class, we will focus on visual specifications using Razor components, with the goal of building the following page:

![Layout](Images/PageLayout.png)

Our classwork will build upon the project you concluded last week, as we work to enhance the layout of the page using Razor components and some basic CSS.



## 1. Create the first Component

1. **Add Buttons**  
   - In a new file named `Actions.razor`, create a `<div>` container to hold your buttons.  
   - Create a corresponding `Actions.razor.css` file and define a `.div-container` class with the following properties:
     ```css
     .div-container {
       display: flex;
       justify-content: space-around;
     }
     ```
   - Add your `Actions` component to the main Razor component by using its HTML tag:
     ```razor
     <Actions />
     ```

2. **Style the Buttons**  
   - Make the buttons round and apply specific colors (e.g., green and grey).  
   - A recommended approach is to define a shared CSS class for the round shape, and separate classes for color variations.  
     - Example: `.round-button`, `.green-button`, `.grey-button`.

### 🏁  Commit Your Changes



## 2. Using Parameters and EventCallbacks

We want the "Start" button to run the same increment logic used in the previous class, without hosting that logic in the `Actions` component itself.

1. **Use `EventCallback`**  
   - In `Actions.razor`, create a parameter of type `EventCallback` to notify the parent component when the "Start" button is clicked. For example:
     ```csharp
     [Parameter]
     public EventCallback StartPressed { get; set; }
     ```
   - In the component's code, create a method to handle the button click event and invoke `StartPressed`.

2. **Handle the Action in the Parent**  
   - In your main component (parent), define a method (e.g., `StartIncrementing`) that increments the user counter.  
   - Pass this method to your `Actions` component:
     ```razor
     <Actions StartPressed="@StartIncrementing" />
     ```
   - Remove or comment out the old button from the previous class, as it is no longer needed.


### 🏁  Commit Your Changes

## 3. Using Images

Next, we need a component to display an image and the current number of users.

1. **Component Setup**  
   - Create `Users.razor` and include an `<img>` tag for the user icon and a text element (like `<span>`) for the counter.  
   - Move the user icon (e.g., `users.png`) into the `wwwroot/images` folder for proper static content usage:
     ```razor
     <img src="images/users.png" alt="Users" />
     <span>@UserCount</span>
     ```
   - Create a parameter to receive the current user count from the parent:
     ```csharp
     [Parameter]
     public int UserCount { get; set; }
     ```
   - In the parent component, pass the counter value to the `Users` component.

2. **Style the Component**  
   - In `Users.razor.css`, create a `.div-container` class with a column layout, and apply it to the top-level `<div>` of your component:
     ```css
     .div-container {
       display: flex;
       flex-direction: column;
       /* additional styling */
     }
     ```
   - Create additional classes to control the image size and text appearance (e.g., font weight, size).

3. **Clean Up Old Display**  
   - Remove or comment out any old display logic (like the counter from the previous class) that’s no longer needed.

### 🏁  Commit Your Changes

## 4. Prepare the Servers Panel Layout

Currently, `Users` and `Actions` take up the entire width. We want to reduce their space and prepare room for a servers panel.

1. **Create a Parent Container**  
   - In your main Razor page, wrap the `Actions` and `Users` components in a `<div>` (parent container).  
   - Inside this parent container, create two child `<div>`s—one for the control panel (containing `Actions` and `Users`) and one for the servers panel (which we will create next).  
   - Apply a display of `flex` to the parent container, set `flex-direction` to `row`, and specify a width for the control panel to reduce its overall size. For example:
     

### 🏁  Commit Your Changes

## 5. Add the Servers

We will track the number of servers and display them in a servers panel.

1. **Create and Configure `Server.razor`**  
   - Create a `Server.razor` component that displays a server image and a label, similar to how you created `Users.razor`.  
   - Use the provided `Server.png` image (place it in your `wwwroot/images` folder).

2. **Increment Servers**  
   - In the main component, declare a variable to hold the server count.  
   - Create a method to handle adding servers (incrementing this count).  
   - Pass this method as a parameter to the `Actions` component (similar to how you did for the `Start` button) if you want to trigger server additions from the same set of actions.

3. **Display Servers**  
   - In the parent (main) component, use a loop to render the correct number of `Server` components:
     ```razor
     @for (int i = 0; i < ServerCount; i++)
     {
       <Server />
     }
     ```
   - Place them inside the servers panel `<div>`.
   - Use the `flex-wrap` property to allow the servers row to have multiple lines

With these steps, you will have:

- A structured Blazor Server application using Razor components (`Actions`, `Users`, and `Server`).
- A layout divided into a **control panel** and a **servers panel**.
- CSS classes to style and position these components appropriately.

### 🏁  Commit Your Changes

## Final Reminder

**⚠️ Don't Forget:** Once you have completed all parts of the assignment, push your code to **this** remote repository.

Good luck, and have fun building your enhanced Blazor application! Use these guidelines and tips to build your solution. Remember, the goal is to experiment, raise questions and learn by doing.