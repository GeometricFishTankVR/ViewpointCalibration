function[X] = Find_Corners(I, win_size, nsx, nsy)
    %This function finds the corners of a checkerboard pattern
    %Input: an image with a checkerboard pattern in it
    %Output:a 2xn array containing the location of the corners
    %Design of the function largely based on the Camera Calibration toolbox


    %Have the user click on the extreme corners of the pattern untill they are
    %satisfied with the results

    sq_corners = [[0 1 1 0];[0 0 1 1]];

    %Determine how many squares are in the pattern
    fprintf('X Squares: %d\nY Suares: %d\n', nsx, nsy)
    sq_p = zeros(2,(nsx+1)*(nsy+1));
    for j = 0:nsy
        for i = 0:nsx
            sq_p(:,j*(nsx+1)+i+1) = [i/nsx;j/nsy];
        end
    end
    im_p = zeros(3,size(sq_p,2));
    
    %Loop through untill user is satisfied
    bad_points = 1;
    while bad_points 
    
        x = zeros(1,4);
        y = zeros(1,4);
    
        % Detect top left corner of the screen, the user will click the corner beside the circle
        % and the best corner is found using cornerfinder (matlab camera calibration toolbox)
        fprintf('Click on the first corner\n')
        [x(1), y(1)] = ginput(1);
        % fuction obtained from (http://www.vision.caltech.edu/bouguetj/calib_doc/)
        [xx] = cornerfinder([x(1);y(1)],I, win_size/2,win_size/2);
        x(1) = xx(1);
        y(1) = xx(2);
        % Plot a creen box around the corner
        plotBox(x(1), y(1), win_size, 'g');

        % Detect top right corner of the screen
        fprintf('Click on the second corner\n')
        [x(2), y(2)] = ginput(1);
        [xx] = cornerfinder([x(2);y(2)],I, win_size/2,win_size/2);
        x(2) = xx(1);
        y(2) = xx(2);
        % Plot a creen box around the corner
        plotBox(x(2), y(2), win_size, 'g');
        plot([x(1), x(2)], [y(1), y(2)], 'g');

        % Detect bottom right corner of the screen
        fprintf('Click on the third corner\n')
        [x(3), y(3)] = ginput(1);
        [xx] = cornerfinder([x(3);y(3)],I, win_size/2,win_size/2);
        x(3) = xx(1);
        y(3) = xx(2);
        % Plot a creen box around the corner
        plotBox(x(3), y(3), win_size, 'g');
        plot([x(2), x(3)], [y(2), y(3)], 'g');

        % Detect bottom left corner of the screen
        fprintf('Click on the fourth corner\n')
        [x(4), y(4)] = ginput(1);
        [xx] = cornerfinder([x(4);y(4)],I, win_size/2,win_size/2);
        x(4) = xx(1);
        y(4) = xx(2);
        plotBox(x(4), y(4), win_size, 'g');
        plot([x(3), x(4)], [y(3), y(4)], 'g');
        % Plot a creen box around the corner
        plot([x(4), x(1)], [y(4), y(1)], 'g');
    
        %Calculate the homography to turn a square into the selected corners
        [H,~,~] = compute_homography([x;y;ones(1,4)],[sq_corners;ones(1,4)]);

        %Calculate the location of each intermediate corner in the pattern
        for i = 1:size(sq_p,2)
            im_p(:,i) = H*[sq_p(:,i);1];
            im_p(:,i) = im_p(:,i)/im_p(3,i);
            [xx] = cornerfinder([im_p(1,i);im_p(2,i)],I, win_size/2,win_size/2);
            im_p(1:2,i) = [xx(1); xx(2)];  
            % Plot a red x on each intermediate corner
            plot(im_p(1,i), im_p(2,i), 'rx')
        end
        % Plot a blue line connecting corners
        plot(im_p(1,:), im_p(2,:), 'b')
    
        % If the points are not good, repeate everything
        a = input('Are the points good?: ','s');
        bad_points = 0;
        if ~isempty(a)
            if ((a == 'n')||(a == 'N'))
                bad_points = 1;
            end
        end
     
    end


X = im_p(1:2,:);


end

function plotBox(x,y,width,c)
%This function plots a box at a specified x and y location with width w and
%colour c
plot([x-width/2, x+width/2],[y+width/2, y+width/2], c)
plot([x-width/2, x+width/2],[y-width/2, y-width/2], c)
plot([x+width/2, x+width/2],[y+width/2, y-width/2], c)
plot([x-width/2, x-width/2],[y+width/2, y-width/2], c)
end
