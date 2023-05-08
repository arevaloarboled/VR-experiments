# This code will help you to plot trajectories results from gam models
library(itsadug)

load("Results.Rdata");

createTracePlot <- function(name, ext, title, model, v, pl, xl, yl, colors, leg, segment){
	
if(ext=='jpg'){
	jpeg(paste(name,ext,sep='.'), res = 300,  width = 7, height = 7, units = 'in')	
} else {
	pdf(paste(name,ext,sep='.'))
}

par(mai = c(1,1,1,0))
plot_smooth(model,
			legend_plot_all = list(x=-1,y=-1), 
			view=v,
			plot_all=pl, rug=F, rm.ranef = T, shade = T,  sim.ci=F,
			xlab = xl, 
			ylab = yl, 
			main=title,
			col = colors,
			hide.label = T,
			lwd = 2,
			cex.axis = 2,
			cex.lab = 2,
			cex.main=2,
 			xaxt="n"
)
axis(1, at = seq(segment, segment +1, by = .2), labels=c('0','.2','.4','.6','.8','1'), 
cex.axis = 2, cex.lab = 2)
legend('topright', legend=leg, col=  colors, lwd=2,  cex = 2, bty='n')

dev.off()	
} 
# replace gm2X with your model
createTracePlot('X_practice','pdf',"", gm2X, "nIdx", "condition", "Normalized time", "X/cm", colorScale, 
c("Rec. front", "Appr. front", "Rec. zenith", "Appr. zenith", "Rec. back", "Appr. back", "Rec. nadir", "Appr. nadir" ),0) 