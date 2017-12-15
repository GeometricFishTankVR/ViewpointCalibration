function plot3grid( T, R, nx, ny, d, c )
%This function plots a grid plane in 3D space
%Input: T is the 3 element translation vector of the top left hand corner 
%           of the grid
%       R is the 3x3 rotation matrix of the grid
%       nx is the number of squares in the x direction
%       ny is the number of squares in the y direction
%       d is the width of each square
%       c is a character denoting the desired colour of the grid

%Andrew Wagemakers
%Dec 7, 2015

nx = nx+1;
ny = ny+1;
grid = zeros(nx,ny,3);

%Calculate the 3D location of each grid point 
for i = 1:nx
    for j = 1:ny
        grid(i,j,:) = T + R*[d*(i-1); d*(j-1); 0];
    end
end

%Plot the horizontal grid lines
for i = 1:nx
   plot3(grid(i,:,1),grid(i,:,2),grid(i,:,3),c) 
end

%Plot the vertical grid lines
for i = 1:ny
   plot3(grid(:,i,1),grid(:,i,2),grid(:,i,3),c) 
end

end
