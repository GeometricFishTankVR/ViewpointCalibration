function plot3cam(T, R, scale, c)
%This function plots a camera frustrum shape in 3D space
%Input: T is the 3 element translation vector of the camera
%       R is the 3x3 rotation matrix of the camera
%       scale is the relative size of the resulting camera
%       c is a character denoting the desired colour of the camera

%Andrew Wagemakers
%Dec 7, 2015

%Calculate the location of the corners of the camera shape relative to the
%center point
relative_corners = scale*[[0.5;0.5;1],[0.5;-0.5;1],[-0.5;-0.5;1],[-0.5;0.5;1]];
corners = zeros(3,5);

%Calculate the location of each corner. the 5th corner is the same as the 
%first one, its calculate such that a completed box will be plotted 
for i = 1:5
    corners(:,i) = T + R*relative_corners(:,mod(i,4)+1);
end

%Plot the corners
plot3(corners(1,:),corners(2,:),corners(3,:),c)

%Plot the lines cnnecting the center of the camera to each corner
for i = 1:4
    plot3([T(1),corners(1,i)],[T(2),corners(2,i)],[T(3),corners(3,i)],c)
end

end
