function plotCalibrationResults(cD, hT, XTD, oV, XDC)
%This function plots the results of the solver to help verify it's success  

    % Plot the calibration points, as well as the transformed tracker
    % points with and without offset applied
    close all;
    figure;
    hold on
    
    numPoints = size(cD, 2);

    vD = zeros(4,numPoints);
    hD = zeros(4,numPoints);    
    for i = 1:numPoints
        oD = XDC(1:3,1:3,i)'*oV;
        hD(:,i) = XTD*hT(:,i);
        vD(:,i) = hD(:,i) - [oD; 0];
    end    
    
    % swap the y and z axis to plot
    hD = hD([1 3 2], :);
    vD = vD([1 3 2], :);
    cD = cD([1 3 2], :);
    for i = 1:numPoints
        plot3([0 cD(1,i)], [0 cD(2,i)], [0 cD(3,i)], 'r');
    end
    
    for i = 1:numPoints
        plot3([0 vD(1,i)], [0 vD(2,i)], [0 vD(3,i)], 'b');
    end
    
    for i = 1:numPoints
        plot3(hD(1, i), hD(2, i), hD(3, i), 'bo');
        plot3([vD(1, i) hD(1,i)], [vD(2, i) hD(2,i)], [vD(3, i) hD(3,i)], 'm');
    end
    xlabel('X axis')
    ylabel('Y axis')
    zlabel('Z axis')
    axis equal
    legend('Calibration Points', 'Transformed + Offset', 'Transformed no Offset')
    hold off   
end