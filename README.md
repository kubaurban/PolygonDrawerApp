# Polygon drawer application

Application written with **.NET 6** using **Windows Forms**.

It allows for drawing polygons and adding relations and constraints for edges. User can also manage drawn polygons, edges and vertices. The application implements the **Model-View-Presenter** architectural pattern.

## Description for user

Application works in four modes:

- **draw**: user can draw points by a left mouse click. At last user should end drawing a polygon by clicking at first drawn point. Drawing lines between points is done using library algorithm by default, but can use implemented *Bresenham Algorithm* in its basic version. To enable *Bresenham Algorithm* user can use a dedicated checkbox.

- **delete**: user can delete any vertex or edge by clicking on them.

- **move**: user can move any vertex or edge by clicking on them and then moving. Polygons also can be moved by clicking and moving dedicated 'move icon' showing up when move mode is chosen.

- **make perpendicular**: user can add perpendicular relation between two selected in this mode edges.

User can manage each edge by right mouse click on it. From the shown up menu user can add length constraint or insert another point in the middle of the edge.
