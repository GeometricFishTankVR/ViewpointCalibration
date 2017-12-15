
% Use this script to calibrate new Cube
clear;
close all;

% Specify the number of corners to be detected
sx = 7;
sy = 5;
warning('off', 'all');

% Get user input 
name = input('Enter the base name of the images: ','s');
num_screen = 3;
d = dir([name '*.jpg']);
I = imread(d(1).name);
win_size = input('Enter the size of the window: ');

% Process first image  
X = Process_Screens(I,win_size, sx-1, sy-1, 3, 1);

top = X(:,:,1);
left = X(:,:,2);
right = X(:,:,3);

% Create variables to store detected corners
top_points = zeros(2,size(top,2),length(d));
left_points = zeros(2,size(left,2),length(d));
right_points = zeros(2,size(left,2),length(d));
points = zeros(size(top,2), 2, num_screen*length(d));
top_points(:,:,1) = top;
left_points(:,:,1) = left;
right_points(:,:,1) = right;
points(:,:,1:3) = cat(3,top', left', right');

% Process each image
for i = 2:length(d)
    I = imread(d(i).name);
    X = Process_Screens(I,win_size, sx-1, sy-1, 3, i);
    top_points(:,:,i) = X(:,:,1);
    left_points(:,:,i) = X(:,:,2);
    right_points(:,:,i) = X(:,:,3);
    points(:,:,(i-1)*3+1:i*3) = cat(3,top_points(:,:,i)', left_points(:,:,i)', right_points(:,:,i)');
end


%%

% Spcify physical dimensions
pixelSize = 96e-6; % 96 micrometer pixel width
resolution = [2048 1536];
p_width = 96e-6;
num_px = 256;
nump = sx*sy;
pwidth = p_width*num_px;
im_points = zeros(2,nump);
kk = length(d);

% Generate location of corners in screen space (maters)
for j = 1:sy
    for i = 1:sx
        im_points(:,sx*(j-1) + i) = [i*pwidth; j*pwidth];
    end
end

% Estimate Camera Patterns
[cP,~,~] = estimateCameraParameters(points,im_points');
[temp_cP,~,~] = estimateCameraParameters(points(:,:,1:3),im_points');
each_cP = struct(temp_cP);
for i = 2:length(d)
    [temp_cP,~,~] = estimateCameraParameters(points(:,:,3*i-2:3*i),im_points');
    each_cP = [each_cP, struct(temp_cP)];
end

%%

n = length(d);
Extrinsic = zeros(4,4,num_screen*n);
Extrinsic(4,:,:) = [zeros(1,3,3*n),ones(1,1,3*n)];


% MATLAB's estimateCameraParameters returns matrices used in right to
% left matrix multiplication. Transpose rotation and translation for
% left to right multiplication
Extrinsic(1:3,1:3,:) = permute(cP.RotationMatrices, [2,1,3]);
Extrinsic(1:3,4,:) = permute(cP.TranslationVectors, [2,3,1]);
    


% Solve for the transformation from each side screen to the top screen
Screen_Transform = zeros(4,4,(num_screen-1)*n);
for i = 1:n
    Screen_Transform(:,:,2*i-1) = Extrinsic(:,:,3*(i-1)+1)\Extrinsic(:,:,3*(i-1)+2);
    Screen_Transform(:,:,2*i) = Extrinsic(:,:,3*(i-1)+1)\Extrinsic(:,:,3*i);   
    fprintf('Image %d successful\n', i);
end

% Use kkn clustering to find 4 transformations
final_Transform = zeros(4,4,4);
each_screen_transform = cell(4,1);
idx = kmeans(squeeze(Screen_Transform(1:3,4,:))',4);
for i = 1:max(idx)
    final_Transform(:,:,i) = mean(Screen_Transform(:,:,idx == i), 3);
    each_screen_transform{i} = Screen_Transform(:,:,idx == i);
end
variance_matrix = zeros(4,4,4);

for i = 1:4
    variance_matrix(:,:,i) = std(each_screen_transform{i},0,3);
end

% Plot the final transformatoins
figure
h = axes;
hold on
set(h,'ydir', 'reverse');
set(h,'zdir', 'reverse');
xlabel('x axis (m)');
ylabel('y axis (m)');
zlabel('z axis (m)');
axis equal
% Plot the top screen in black
plot3screen([0;0;0], eye(3),resolution(1)*p_width, resolution(2)*p_width, pwidth, 'k');
plot3cam([0;0;0], eye(3), 0.01, 'k');
colors = ['b', 'r', 'm', 'g', 'c', 'y'];
% Plot the side screens in random colors
for i = 1:4
    plot3screen(final_Transform(1:3,4,i), final_Transform(1:3,1:3,i), resolution(1)*p_width,resolution(2)*p_width, pwidth, colors(mod(i,6)));
    plot3cam(final_Transform(1:3,4,i), final_Transform(1:3,1:3,i),0.01, colors(mod(i,6)));
end

% Plot each transformation (Usefull to see how precise the results are)
figure
h = axes;
hold on
set(h,'ydir', 'reverse');
set(h,'zdir', 'reverse');
axis equal
plot3screen([0;0;0], eye(3),resolution(1)*pixelSize, resolution(2)*p_width, pwidth, 'k');
plot3cam([0;0;0], eye(3), 0.01, 'k');
colors = ['b', 'r', 'm', 'g', 'c', 'y'];
for i = 1:n
    plot3screen(Screen_Transform(1:3,4,i), Screen_Transform(1:3,1:3,i), resolution(1)*p_width,resolution(2)*p_width, pwidth, colors(mod(i,6)+1));
    plot3cam(Screen_Transform(1:3,4,i), Screen_Transform(1:3,1:3,i),0.01, colors(mod(i,6)+1));
end

% Save results to make it easy to plot later
save('ScreenCalibrationInfo');

for i = 1:4
    save_matrix_to_xml(final_Transform(:,:,i), ['transform_' int2str(i)]);
end

